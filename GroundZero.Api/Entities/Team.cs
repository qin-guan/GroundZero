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
    public required string Name { get; set; }

    /// <summary>
    /// Team / project description
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Team / project location
    /// </summary>
    public required string Location { get; set; }

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
    public bool Active { get; set; } = true;
    public bool Prioritized { get; set; } = false;
    public double Mu { get; set; } = CrowdBt.MuPrior;
    public double SigmaSq { get; set; } = CrowdBt.SigmaSqPrior;

    public Guid? NextJudgeId { get; set; }
    public Judge? NextJudge { get; set; }

    public Guid? PreviousJudgeId { get; set; }
    public Judge? PreviousJudge { get; set; }

    public ICollection<Judge> JudgesIgnored { get; set; } = [];
    public ICollection<Judge> JudgesViewed { get; set; } = [];
    public ICollection<Participant> Members { get; set; } = [];

    public Guid HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }
}