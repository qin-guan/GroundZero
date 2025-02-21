namespace GroundZero.Api.Entities;

public abstract class HackathonUser
{
    public Guid Id { get; set; }

    public Guid HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; }
}
