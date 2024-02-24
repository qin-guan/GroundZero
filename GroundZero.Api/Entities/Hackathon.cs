namespace GroundZero.Api.Entities;

public class Hackathon
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Venue { get; set; }

    public DateTimeOffset EventStartDateTime { get; set; }
    public DateTimeOffset EventEndDateTime { get; set; }
    public DateTimeOffset RegistrationStartDateTime { get; set; }
    public DateTimeOffset RegistrationEndDateTime { get; set; }

    public User Organizer { get; set; }

    public List<Team> Teams { get; set; }
    public List<Judge> Judges { get; set; }
}