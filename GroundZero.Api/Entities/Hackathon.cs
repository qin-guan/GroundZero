namespace GroundZero.Api.Entities;

public class Hackathon
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Venue { get; set; }
    public required Uri HomepageUri { get; set; }

    public ICollection<HackathonUser> Users { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
}
