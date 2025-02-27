using FastEndpoints;
using GroundZero.Api.Context;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace GroundZero.Api.Endpoints.Hackathon.Judge.NextTeam;

public class GetNextTeamEndpoint(AppDbContext dbContext, UserManager<AppUser> userManager) : Endpoint<GetNextTeamRequest, Team>
{
    public override void Configure()
    {
        Get("/Hackathon/{Id:guid}/Judge/NextTeam");
    }

    public override async Task HandleAsync(GetNextTeamRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(userManager.GetUserId(User));

        var judge = await dbContext.Judges
            .Include(j => j.Hackathon)
            .ThenInclude(h => h.Teams)
            .Include(j => j.ViewedTeams)
            .Include(j => j.PreviousTeam)
            .Include(j => j.NextTeam)
            .Include(j => j.IgnoredTeams)
            .SingleAsync(j => j.UserId == userId && j.HackathonId == req.Id);

        if (judge.NextTeam is null)
        {
            var availableItems = judge.Hackathon.Teams
                .Where(t => t.Active)
                .Where(t => judge.IgnoredTeams.All(st => st.Id != t.Id))
                .ToList();

            var items = availableItems.Any(i => i.Prioritized)
                ? availableItems.Where(i => i.Prioritized).ToList()
                : availableItems;

            var busyProjects = (await dbContext.Judges
                .Where(j => j.HackathonId == req.Id)
                .Where(j => j.NextTeamId != null)
                .ToListAsync())
                .Where(j => (DateTimeOffset.UtcNow - j.UpdatedAt) < TimeSpan.FromSeconds(60))
                .Select(j => j.NextTeamId);

            var nonBusyProjects = items.Where(i => !busyProjects.Contains(i.Id)).ToList();

            var preferred = nonBusyProjects.Count != 0 ? nonBusyProjects : items;
            var lessSeen = preferred.Where(t => t.JudgesViewed.Count < 3).ToList();

            var preferredItems = lessSeen.Count != 0 ? lessSeen : preferred;
            judge.NextTeam = preferredItems.Shuffle().FirstOrDefault();
            if (judge.NextTeam is not null)
            {
                judge.NextTeam.Prioritized = false;
            }

            judge.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(ct);
        }

        if (judge.NextTeam is null)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendAsync(judge.NextTeam, cancellation: ct);
    }
}
