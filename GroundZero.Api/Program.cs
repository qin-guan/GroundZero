using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using GroundZero.Api.Context;
using GroundZero.Api.Endpoints;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>("groundzero");

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
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await userManager.CreateAsync(new AppUser
    {
        Email = "admin@admin.com",
        UserName = "admin@admin.com",
        EmailConfirmed = true
    }, "QinGuan12345!");

    await userManager.CreateAsync(new AppUser
    {
        Email = "participant@admin.com",
        UserName = "participant@admin.com",
        EmailConfirmed = true
    }, "QinGuan12345!");

    await userManager.CreateAsync(new AppUser
    {
        Email = "judge@admin.com",
        UserName = "judge@admin.com",
        EmailConfirmed = true
    }, "QinGuan12345!");

    var organizer = await db.Users.SingleAsync(u => u.Email == "admin@admin.com");
    var participant = await db.Users.SingleAsync(u => u.Email == "participant@admin.com");
    var judge = await db.Users.SingleAsync(u => u.Email == "judge@admin.com");

    var hackomania = await db.Hackathons.AddAsync(new Hackathon
    {
        Id = new Guid("019508ec-6f4c-7bf7-9793-1adf50f067be"),
        Name = "HackOMania",
        Description = "",
        Venue = "",
        HomepageUri = new Uri("https://hackomania.geekshacking.com"),
        Organizer = organizer,
        Teams =
        [
            new Team
            {
                Name = "Team 1",
                Description = "",
                Location = "",
                Members =
                [
                    new Participant
                    {
                        User = participant
                    }
                ]
            },
            new Team
            {
                Name = "Team 2",
                Description = "",
                Location = "",
                Members =
                [
                    new Participant
                    {
                        User = participant
                    }
                ]
            },
            new Team
            {
                Name = "Team 3",
                Description = "",
                Location = "",
                Members =
                [
                    new Participant
                    {
                        User = participant
                    }
                ]
            }
        ],
        Judges =
        [
            new Judge
            {
                User = judge,
            }
        ]
    });

    await db.SaveChangesAsync();
}

app.UseHttpsRedirection();

app.UseFastEndpoints();
app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference();
app.MapIdentityApi<AppUser>();

app.Run();