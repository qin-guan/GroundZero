using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Groundzero.Api.Context;

public static class DataSeedingExtesions
{
    public static void AddSeedData(this DbContext dbContext)
    {
        var hasher = new PasswordHasher<AppUser>();

        if (dbContext.Set<AppUser>().Any()) return;

        var admin = new AppUser
        {
            Email = "admin@groundzero.dev",
            NormalizedEmail = "ADMIN@GROUNDZERO.DEV",
            UserName = "admin@groundzero.dev",
            NormalizedUserName = "ADMIN@GROUNDZERO.DEV",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin");

        var judge = new AppUser
        {
            Email = "judge@groundzero.dev",
            NormalizedEmail = "JUDGE@GROUNDZERO.DEV",
            UserName = "judge@groundzero.dev",
            NormalizedUserName = "JUDGE@GROUNDZERO.DEV",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        judge.PasswordHash = hasher.HashPassword(judge, "admin");

        var participant1 = new Participant
        {
            User = new AppUser
            {
                Email = "participant1@groundzero.dev",
                NormalizedEmail = "PARTICIPANT1@GROUNDZERO.DEV",
                UserName = "participant1@groundzero.dev",
                NormalizedUserName = "PARTICIPANT1@GROUNDZERO.DEV",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };
        participant1.User.PasswordHash = hasher.HashPassword(participant1.User, "admin");

        var participant2 = new Participant
        {
            User = new AppUser
            {
                Email = "participant2@groundzero.dev",
                NormalizedEmail = "PARTICIPANT2@GROUNDZERO.DEV",
                UserName = "participant2@groundzero.dev",
                NormalizedUserName = "PARTICIPANT2@GROUNDZERO.DEV",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };
        participant2.User.PasswordHash = hasher.HashPassword(participant2.User, "admin");

        var participant3 = new Participant
        {
            User = new AppUser
            {
                Email = "participant3@groundzero.dev",
                NormalizedEmail = "PARTICIPANT3@GROUNDZERO.DEV",
                UserName = "participant3@groundzero.dev",
                NormalizedUserName = "PARTICIPANT3@GROUNDZERO.DEV",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };
        participant3.User.PasswordHash = hasher.HashPassword(participant1.User, "admin");

        dbContext.Set<AppUser>().AddRange(admin, judge);
        dbContext.Set<Participant>().AddRange(participant1, participant2, participant3);

        var hackathonId = Guid.NewGuid();
        var hackathon = new Hackathon
        {
            Id = hackathonId,
            Name = "HackOMania 2025",
            Description = "",
            Venue = "",
            HomepageUri = new Uri("https://hackomania.geekshacking.com"),
            Users = [
                new Organizer 
                {
                    User = admin,
                    OrganizerType = OrganizerType.Admin
                },
                new Judge
                {
                    User = judge
                },
                participant1,
                participant2,
                participant3
            ],
            Teams =
            [
                new Team
                     {
                         Name = "Team 1",
                         Description = "",
                         Location = "",
                         Members =
                         [
                            participant1
                         ]
                     },
                     new Team
                     {
                         Name = "Team 2",
                         Description = "",
                         Location = "",
                         Members =
                         [
                            participant2
                         ]
                     },
                     new Team
                     {
                         Name = "Team 3",
                         Description = "",
                         Location = "",
                         Members =
                         [
                            participant3
                         ]
                     }
            ]
        };

        dbContext.Set<Hackathon>().Add(hackathon);
    }
}
