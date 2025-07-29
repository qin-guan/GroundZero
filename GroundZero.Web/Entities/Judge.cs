using System.Security.Cryptography;
using GroundZero.Gavel;
using SqlSugar;

namespace GroundZero.Web.Entities;

public class Judge
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public string Secret { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    #region CrowdBT

    [SugarColumn(IsNullable = true)]
    public DateTimeOffset? UpdatedAt { get; set; }

    public double Alpha { get; set; } = CrowdBt.AlphaPrior;

    public double Beta { get; set; } = CrowdBt.BetaPrior;

    #endregion

    [SugarColumn(IsNullable = true)]
    public Guid? NextTeamId { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(NextTeamId))]
    public Team? NextTeam { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? PreviousTeamId { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(PreviousTeamId))]
    public Team? PreviousTeam { get; set; }

    public Guid HackathonId { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Decision.JudgeId))]
    public List<Decision> Decisions { get; set; } = null!;

    [Navigate(typeof(TeamJudgeIgnored), nameof(TeamJudgeIgnored.JudgeId), nameof(TeamJudgeIgnored.TeamId))]
    public List<Team> IgnoredTeams { get; set; } = null!;

    [Navigate(typeof(TeamJudgeViewed), nameof(TeamJudgeViewed.JudgeId), nameof(TeamJudgeViewed.TeamId))]
    public List<Team> ViewedTeams { get; set; } = null!;
}