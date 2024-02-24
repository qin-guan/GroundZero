namespace GroundZero.Api.Entities;

public class Challenge
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid HackathonId { get; set; }

    public Hackathon Hackathon { get; set; }

    public List<Team> Teams { get; set; }
    public List<Judge> Judges { get; set; }
}