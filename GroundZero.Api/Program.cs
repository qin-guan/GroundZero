using FastEndpoints;
using FastEndpoints.Swagger;
using Groundzero.Api.Context;
using GroundZero.Api.Context;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>(
    "groundzero",
    configureDbContextOptions: options =>
    {
        options.UseSeeding((context, _) =>
        {
            context.AddSeedData();
            context.SaveChanges();
        });

        // options.UseAsyncSeeding(async (context, _, ct) =>
        // {
        //     context.AddSeedData();
        //     await context.SaveChangesAsync();
        // });
    });

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // await db.Database.EnsureDeletedAsync();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseFastEndpoints();
app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference();
app.MapIdentityApi<AppUser>();

app.Run();
