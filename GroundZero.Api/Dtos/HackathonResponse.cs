namespace GroundZero.Api.Dtos;

public class HackathonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Venue { get; set; }
    public Uri HomepageUri { get; set; }
}
