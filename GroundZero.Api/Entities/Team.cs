using System.Security.Cryptography;
using GroundZero.Gavel;

namespace GroundZero.Api.Entities;

/// <summary>
/// Team and project information
/// </summary>
public class Team
{
    public Guid Id { get; set; }

    /// <summary>
    /// Team / project name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Team / project description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Team / project location
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Project Devpost link
    /// </summary>
    public Uri? DevpostUri { get; set; }

    /// <summary>
    /// Project code repository link
    /// </summary>
    public Uri? RepositoryUri { get; set; }

    /// <summary>
    /// Secret code to join team
    /// </summary>
    public string JoinCode { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    // CrowedBT judging
    public bool Active { get; set; }
    public bool Prioritized { get; set; }
    public double Mu { get; set; } = CrowdBt.MuPrior;
    public double SigmaSq { get; set; } = CrowdBt.SigmaSqPrior;

    public Guid? NextJudgeId { get; set; }
    public Judge? NextJudge { get; set; }
    public Guid? PreviousJudgeId { get; set; }
    public Judge? PreviousJudge { get; set; }

    public List<Judge> SkippedJudges { get; set; }
    public List<User> Members { get; set; }

    public Guid? ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }
}