using SqlSugar;

namespace GroundZero.Web.Entities;

public class Hackathon
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Venue { get; set; } = null!;

    public string HomepageUri { get; set; } = null!;

    [Navigate(NavigateType.OneToMany, nameof(Participant.HackathonId))]
    public List<Participant> Participants { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Organizer.HackathonId))]
    public List<Organizer> Organizers { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Team.HackathonId), nameof(Id))]
    public List<Team> Teams { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Resource.HackathonId))]
    public List<Resource> Resources { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Judge.HackathonId))]
    public List<Judge> Judges { get; set; }
}