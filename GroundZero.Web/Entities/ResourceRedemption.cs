using SqlSugar;

namespace GroundZero.Web.Entities;

public class ResourceRedemption
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Guid ResourceId { get; set; }

    public Guid RedeemerId { get; set; }
}