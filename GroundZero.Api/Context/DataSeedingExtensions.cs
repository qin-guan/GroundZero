using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Groundzero.Api.Context;

public static class DataSeedingExtesions
{
    public static void AddSeedData(this DbContext dbContext)
    {
        var hasher = new PasswordHasher<AppUser>();
        
        if (dbContext.Set<AppUser>().SingleOrDefault(u => u.Email == "admin@groundzero.dev") is null)
        {
        var admin = new AppUser
        {
            Email = "admin@groundzero.dev",
            UserName = "admin@groundzero.dev",
            EmailConfirmed = true,
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin");
        
        dbContext.Set<AppUser>().Add(admin);
        }

        // var judge = new AppUser
        // {
        //     Email = "judge@groundzero.dev",
        //     UserName = "judge@groundzero.dev",
        //     EmailConfirmed = true,
        // };
        // judge.PasswordHash = hasher.HashPassword(judge, "admin");

        // var participant1 = new AppUser
        // {
        //     Email = "participant1@groundzero.dev",
        //     UserName = "participant1@groundzero.dev",
        //     EmailConfirmed = true,
        // };
        // participant1.PasswordHash = hasher.HashPassword(participant1, "admin");

        // var participant2 = new AppUser
        // {
        //     Email = "participant2@groundzero.dev",
        //     UserName = "participant2@groundzero.dev",
        //     EmailConfirmed = true,
        // };
        // participant2.PasswordHash = hasher.HashPassword(participant2, "admin");

        // var participant3 = new AppUser
        // {
        //     Email = "participant3@groundzero.dev",
        //     UserName = "participant3@groundzero.dev",
        //     EmailConfirmed = true,
        // };
        // participant3.PasswordHash = hasher.HashPassword(participant3, "admin");

        // dbContext.Set<AppUser>().AddRange(admin, judge, participant1, participant2, participant3);

        // var hackathon = new Hackathon
        // {
        //     Name = "HackOMania 2025",
        //     Description = "",
        //     Venue = "",
        //     HomepageUri = new Uri("https://hackomania.geekshacking.com"),
        //     Organizers = [
        //         new Organizer {
        //                 OrganizerType = OrganizerType.Admin,
        //             }
        //     ],
        //     Teams =
        //     [
        //         new Team
        //             {
        //                 Name = "Team 1",
        //                 Description = "",
        //                 Location = "",
        //                 Members =
        //                 [
        //                     new Participant
        //                     {
        //                         User = participant1,
        //                     }
        //                 ]
        //             },
        //             new Team
        //             {
        //                 Name = "Team 2",
        //                 Description = "",
        //                 Location = "",
        //                 Members =
        //                 [
        //                     new Participant
        //                     {
        //                         User = participant2
        //                     }
        //                 ]
        //             },
        //             new Team
        //             {
        //                 Name = "Team 3",
        //                 Description = "",
        //                 Location = "",
        //                 Members =
        //                 [
        //                     new Participant
        //                     {
        //                         User = participant3
        //                     }
        //                 ]
        //             }
        //     ]
        // };

        // dbContext.Set<Hackathon>().Add(hackathon);

    }
}
