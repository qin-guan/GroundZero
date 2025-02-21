using FastEndpoints;
using Groundzero.Api.Mappers;
using GroundZero.Api.Context;
using GroundZero.Api.Dtos;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GroundZero.Api.Endpoints.Hackathon;

public class ListHackathonEndpoint(UserManager<AppUser> userManager, AppDbContext dbContext) : EndpointWithoutRequest<IEnumerable<HackathonResponse>>
{
    public override void Configure()
    {
        Get("/Hackathon");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = userManager.GetUserId(User);
        ArgumentNullException.ThrowIfNull(userId);

        var user = await dbContext.Users
        .Include(u => u.HackathonsAdminIn)
        .Include(u => u.HackathonsJudgedIn)
          .ThenInclude(j => j.Hackathon)
        .Include(u => u.HackathonsParticipatedIn)
          .ThenInclude(p => p.Team)
            .ThenInclude(t => t!.Hackathon)
        .SingleOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        ArgumentNullException.ThrowIfNull(user);

        var hackathons = user.HackathonsAdminIn
        .Concat(user.HackathonsJudgedIn.Select(j => j.Hackathon))
        .Concat(
          user.HackathonsParticipatedIn
          .Where(p => p.Team is not null)
          .Select(p => p.Team!.Hackathon)
        );

        await SendOkAsync(
          hackathons.ToResponse(),
          ct
        );
    }
}
