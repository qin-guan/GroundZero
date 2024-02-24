using Microsoft.AspNetCore.Identity;

namespace GroundZero.Api.Entities;

public class User : IdentityUser<Guid>
{
    public List<Team> Teams { get; set; }

    public List<Hackathon> HackathonJudgeAssignments { get; set; }
    public List<Judge> JudgeAssignments { get; set; }
}