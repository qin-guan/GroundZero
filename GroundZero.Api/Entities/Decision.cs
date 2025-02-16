namespace GroundZero.Api.Entities;

public class Decision
{
    public Guid Id { get; set; }

    public Guid JudgeId { get; set; }
    public Judge Judge { get; set; }

    public Guid WinnerId { get; set; }
    public Team Winner { get; set; }
    public Guid LoserId { get; set; }
    public Team Loser { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}