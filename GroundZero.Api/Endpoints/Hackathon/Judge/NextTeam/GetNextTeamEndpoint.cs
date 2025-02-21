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
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: ct);

        var hackathon = await dbContext.Hackathons
            .Include(h => h.Teams)
            .ThenInclude(t => t.JudgesViewed)
            .Include(h => h.Judges)
            .ThenInclude(j => j.NextTeam)
            .Include(h => h.Judges)
            .ThenInclude(j => j.PreviousTeam)
            .Include(h => h.Judges)
            .ThenInclude(j => j.IgnoredTeams)
            .SingleOrDefaultAsync(h => h.Id == req.Id, cancellationToken: ct);
        ArgumentNullException.ThrowIfNull(hackathon);

        var judge = hackathon.Judges.SingleOrDefault(j => j.UserId == user.Id);
        if (judge is null)
        {
            throw new Exception("User is not a judge.");
        }

        if (judge.NextTeam is null)
        {
            var availableItems = hackathon.Teams
                .Where(t => t.Active)
                .Where(t => judge.IgnoredTeams.All(st => st.Id != t.Id))
                .ToList();

            var items = availableItems.Any(i => i.Prioritized)
                ? availableItems.Where(i => i.Prioritized).ToList()
                : availableItems;

            var busyProjects = hackathon.Judges
                .Where(j => j.NextTeamId != null)
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