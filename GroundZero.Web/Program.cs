using AspNet.Security.OAuth.GitHub;
using FastEndpoints;
using FastEndpoints.Swagger;
using GroundZero.Web.Authentication;
using GroundZero.Web.Components;
using GroundZero.Web.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

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

builder.Services.AddTransient<IClaimsTransformation, GitHubClaimsTransformer>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options => { options.LoginPath = "/Auth/GitHub/Login"; })
    .AddGitHub(options =>
    {
        options.ClientId = "Iv23lieaI0YRJGwxCDUb";
        options.ClientSecret = "";
        options.CallbackPath = "/Auth/GitHub/Callback";
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
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GroundZero.Web.Client._Imports).Assembly);

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints();

app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference();
// app.MapIdentityApi<AppUser>();

app.Run();