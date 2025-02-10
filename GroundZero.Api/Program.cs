using GroundZero.Api.Context;
using GroundZero.Api.Endpoints;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>("groundzero");

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
}

app.UseHttpsRedirection();

app.MapIdentityApi<IdentityUser>();

var hackathons = app.MapGroup("/v1/hackathons");
hackathons.MapGet("/", HackathonEndpoints.GetHackathon);

app.Run();