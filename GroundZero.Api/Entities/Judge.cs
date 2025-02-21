using System.Security.Cryptography;
using GroundZero.Gavel;

namespace GroundZero.Api.Entities;

public class Judge : HackathonUser
{
    public string Secret { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    // CrowdBT judging
    public DateTimeOffset UpdatedAt { get; set; }
    public double Alpha { get; set; } = CrowdBt.AlphaPrior;
    public double Beta { get; set; } = CrowdBt.BetaPrior;

    public Guid? NextTeamId { get; set; }
    public Team? NextTeam { get; set; }

    public Guid? PreviousTeamId { get; set; }
    public Team? PreviousTeam { get; set; }

    public ICollection<Team> IgnoredTeams { get; set; } = [];
    public ICollection<Team> ViewedTeams { get; set; } = [];
}
