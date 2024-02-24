using System.Security.Cryptography;
using GroundZero.Gavel;

namespace GroundZero.Api.Entities;

public class Judge
{
    public Guid Id { get; set; }

    public string Secret { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid ChallengeId { get; set; }
    public Challenge Challenge { get; set; }

    // CrowdBT judging
    public double Alpha { get; set; } = CrowdBt.AlphaPrior;
    public double Beta { get; set; } = CrowdBt.BetaPrior;

    public Guid? NextTeamId { get; set; }
    public Team? NextTeam { get; set; }

    public Guid? PreviousTeamId { get; set; }
    public Team? PreviousTeam { get; set; }

    public List<Team> SkippedTeams { get; set; }
}