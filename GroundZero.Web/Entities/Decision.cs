using SqlSugar;

namespace GroundZero.Web.Entities;

public class Decision
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public Guid JudgeId { get; set; }

    [Navigate(NavigateType.ManyToOne, nameof(JudgeId))]
    public Judge Judge { get; set; } = null!;

    public Guid WinnerId { get; set; }

    [Navigate(NavigateType.ManyToOne, nameof(WinnerId))]
    public Team Winner { get; set; } = null!;

    public Guid LoserId { get; set; }

    [Navigate(NavigateType.ManyToOne, nameof(LoserId))]
    public Team Loser { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}