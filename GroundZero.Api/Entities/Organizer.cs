namespace GroundZero.Api.Entities;

public class Organizer : HackathonUser
{
    public required OrganizerType OrganizerType { get; set; }
}
