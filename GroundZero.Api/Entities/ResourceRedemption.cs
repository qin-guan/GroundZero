namespace GroundZero.Api.Entities;

public class ResourceRedemption
{
    public Guid Id { get; set; }

    public DateTimeOffset RedeemedAt { get; set; } = DateTimeOffset.UtcNow;

    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; }

    public Guid RedeemerId { get; set; }
    public HackathonUser Redeemer { get; set; }
}
