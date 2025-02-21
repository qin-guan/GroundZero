namespace GroundZero.Api.Entities;

public class Participant : HackathonUser
{
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }
}
