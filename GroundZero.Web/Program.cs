using System.Security.Claims;
using AspNet.Security.OAuth.GitHub;
using FastEndpoints;
using FastEndpoints.Swagger;
using GroundZero.Web.Components;
using GroundZero.Web.Context;
using GroundZero.Web.Entities;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHxServices();
builder.Services.AddHxMessenger();
builder.Services.AddHxMessageBoxHost();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddSingleton<ISqlSugarClient>(sp =>
{
    var sqlSugar = new SqlSugarScope(new ConnectionConfig
        {
            DbType = DbType.MySql,
            ConnectionString = builder.Configuration.GetConnectionString("groundzero"),
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityNameService = (_, entity) => { entity.IsDisabledDelete = true; }
            }
        },
        db =>
        {
            db.Aop.OnLogExecuting = (sql, _) =>
            {
                var logger = sp.GetService<ILogger<ISqlSugarClient>>();
                logger?.LogInformation("SQL: {Sql}", sql);
            };
        });

    return sqlSugar;
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/GitHub/Login";
        options.Cookie.Name = ".AspNet.GroundZero";

        if (builder.Environment.IsProduction())
        {
            options.Cookie.Domain = ".from.sg";
        }
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["GitHub:ClientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration["GitHub:ClientSecret"] ?? string.Empty;
        options.CallbackPath = "/Auth/GitHub/Callback";

        options.Events.OnCreatingTicket = async context =>
        {
            var userName = context.Principal?.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            ArgumentNullException.ThrowIfNull(userName);

            var db = context.HttpContext.RequestServices.GetRequiredService<ISqlSugarClient>();

            var account = await db.Queryable<GitHubOnlineAccount>()
                .Includes(a => a.User)
                .SingleAsync(a => a.UserName == userName);

            if (account is null)
            {
                account = await db.InsertNav(new GitHubOnlineAccount
                    {
                        UserName = userName,
                        User = new User { Name = userName }
                    })
                    .Include(g => g.User)
                    .ExecuteReturnEntityAsync();
            }

            context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()));
        };
    });

builder.Services
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var client = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    await client.AddSeedDataAsync();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GroundZero.Web.Client._Imports).Assembly);

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints();

app.UseAntiforgery();

app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference();
app.MapGet("/auth/github/login",
    async (context) =>
    {
        await context.ChallengeAsync(new AuthenticationProperties
        {
            RedirectUri = "/home"
        });
    });
app.MapGet("/auth/logout",
    async (context) =>
    {
        await context.SignOutAsync(new AuthenticationProperties
        {
            RedirectUri = "/"
        });
    });

app.Run();