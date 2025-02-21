namespace GroundZero.Api.Entities;

public class Resource
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    public required string Description { get; set; }

    public Guid HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }
}
