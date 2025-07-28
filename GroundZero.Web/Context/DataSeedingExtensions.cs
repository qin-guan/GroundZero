using GroundZero.Web.Entities;
using SqlSugar;

namespace GroundZero.Web.Context;

public static class DataSeedingExtensions
{
    public static async Task AddSeedDataAsync(this ISqlSugarClient client)
    {
        client.DbMaintenance.CreateDatabase();

        var types = typeof(User).Assembly.GetTypes()
            .Where(it => it.FullName is not null && it.FullName.Contains("GroundZero.Web.Entities") &&
                         it is { IsClass: true, IsAbstract: false })
            .ToArray();

        client.CodeFirst.InitTables(types);

        if (!await client.Queryable<User>().AnyAsync())
        {
            var qg = await client
                .InsertNav(
                    new GitHubOnlineAccount
                    {
                        Id = Guid.NewGuid(),
                        User = new User
                        {
                            Name = "Qin Guan",
                        },
                        UserName = "qin-guan"
                    }
                )
                .Include(g => g.User)
                .ExecuteReturnEntityAsync();

            var hackathon = await client.Insertable(new Hackathon
                {
                    Name = "HackOMania 2026",
                    ShortCode = "hackomania2026",
                    Description = "The largest student hackathon in Singapore",
                    Venue = "Somewhere over the rainbow",
                    HomepageUri = "https://hackomania.geekshacking.com",
                })
                .ExecuteReturnEntityAsync();

            var team = await client.Insertable(new Team
                {
                    Name = "Qin Guan's Team",
                    HackathonId = hackathon.Id
                })
                .ExecuteReturnEntityAsync();

            var qgParticipant = await client.InsertNav(new Participant
                {
                    TeamId = team.Id,
                    UserId = qg.Id,
                    ParticipantReviews =
                    [
                        new ParticipantReview
                        {
                            Status = ParticipantReviewStatus.Accepted,
                            Reason = "Great guy.",
                        }
                    ],
                })
                .Include(p => p.ParticipantReviews)
                .ExecuteReturnEntityAsync();
        }
    }
}