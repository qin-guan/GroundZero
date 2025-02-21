using FastEndpoints;
using GroundZero.Api.Context;
using GroundZero.Api.Entities;
using GroundZero.Gavel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace GroundZero.Api.Endpoints.Hackathon.Judge.Vote;

public class PostVoteEndpoint(UserManager<AppUser> userManager, AppDbContext dbContext) : Endpoint<PostVoteRequest>
{
    public override void Configure()
    {
        Post("/Hackathon/{Id:guid}/Judge/Vote");
    }

    public override async Task HandleAsync(PostVoteRequest req, CancellationToken ct)
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

        ArgumentNullException.ThrowIfNull(judge.NextTeam);

        if (req.Action == "Skip")
        {
            judge.IgnoredTeams.Add(judge.NextTeam);
            // judge.NextTeamId = null;
            Console.WriteLine("here");
        }
        else
        {
            Console.WriteLine("Here2");
            if (judge.PreviousTeam is null)
            {
                judge.IgnoredTeams.Add(judge.NextTeam);
                judge.PreviousTeam = judge.NextTeam;
            }
            else
            {
                if (judge.PreviousTeam.Active && judge.NextTeam.Active)
                {
                    var winner = req.Action == "Previous" ? judge.PreviousTeam : judge.NextTeam;
                    var loser = req.Action == "Previous" ? judge.NextTeam : judge.PreviousTeam;

                    var (uAlpha, uBeta, uWinnerMu, uWinnerSigmaSq, uLoserMu, uLoserSigmaSq) = CrowdBt.Update(
                        judge.Alpha,
                        judge.Beta,
                        winner.Mu,
                        winner.SigmaSq,
                        loser.Mu,
                        loser.SigmaSq
                    );

                    judge.Alpha = uAlpha;
                    judge.Beta = uBeta;
                    winner.Mu = uWinnerMu;
                    winner.SigmaSq = uWinnerSigmaSq;
                    loser.Mu = uLoserMu;
                    loser.SigmaSq = uLoserSigmaSq;

                    await dbContext.Decisions.AddAsync(new Decision
                    {
                        Judge = judge,
                        Winner = winner,
                        Loser = loser
                    }, ct);
                }

                judge.NextTeam.JudgesViewed.Add(judge);
                judge.PreviousTeam = judge.NextTeam;
                judge.IgnoredTeams.Add(judge.PreviousTeam);
            }
        }

        await dbContext.SaveChangesAsync(ct);

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
        var teams = preferredItems.Shuffle().ToList();
        if (teams.Count == 0)
        {
            Console.WriteLine("here3");
            judge.NextTeamId = null;
        }
        else
        {
            if (new Random().NextDouble() < CrowdBt.Epsilon)
            {
                judge.NextTeam = teams.FirstOrDefault();
            }
            else
            {
                judge.NextTeam =
                    CrowdBt.Argmax(
                        i => CrowdBt.ExpectedInformationGain(
                            judge.Alpha,
                            judge.Beta,
                            judge.PreviousTeam.Mu,
                            judge.PreviousTeam.SigmaSq,
                            i.Mu,
                            i.SigmaSq
                        ),
                        teams
                    );
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}