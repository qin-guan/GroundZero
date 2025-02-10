using System.Security.Cryptography;
using GroundZero.Gavel;

namespace GroundZero.Api.Entities;

public class Judge
{
    public Guid Id { get; set; }

    public string Secret { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    // CrowdBT judging
    public double Alpha { get; set; } = CrowdBt.AlphaPrior;
    public double Beta { get; set; } = CrowdBt.BetaPrior;

    public Guid UserId { get; set; }
    public AppUser User { get; set; }
    
    public Guid HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }

    public Guid? NextTeamId { get; set; }
    public Team? NextTeam { get; set; }

    public Guid? PreviousTeamId { get; set; }
    public Team? PreviousTeam { get; set; }

    public ICollection<Team> SkippedTeams { get; set; } = [];
}