using System.Security.Cryptography;
using GroundZero.Gavel;
using SqlSugar;

namespace GroundZero.Web.Entities;

/// <summary>
/// Team and project information
/// </summary>
public class Team
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    /// <summary>
    /// Team / project name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Team / project description
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// Team / project location
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string? Location { get; set; }

    /// <summary>
    /// Project Devpost link
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string? DevpostUri { get; set; }

    /// <summary>
    /// Project code repository link
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string? RepositoryUri { get; set; }

    /// <summary>
    /// Secret code to join team
    /// </summary>
    public string JoinCode { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    #region CrowdBT

    public bool Active { get; set; } = true;

    public bool Prioritized { get; set; } = false;

    public double Mu { get; set; } = CrowdBt.MuPrior;

    public double SigmaSq { get; set; } = CrowdBt.SigmaSqPrior;

    #endregion


    public Guid HackathonId { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Participant.TeamId))]
    public List<Participant> Members { get; set; } = null!;

    [Navigate(NavigateType.OneToMany, nameof(Decision.WinnerId))]
    public List<Decision> WonDecisions { get; set; } = null!;

    [Navigate(NavigateType.OneToMany, nameof(Decision.LoserId))]
    public List<Decision> LostDecisions { get; set; } = null!;

    [Navigate(typeof(TeamJudgeIgnored), nameof(TeamJudgeIgnored.JudgeId), nameof(TeamJudgeIgnored.TeamId))]
    public List<Judge> JudgesIgnored { get; set; } = null!;

    [Navigate(typeof(TeamJudgeViewed), nameof(TeamJudgeViewed.JudgeId), nameof(TeamJudgeViewed.TeamId))]
    public List<Judge> JudgesViewed { get; set; } = null!;
}