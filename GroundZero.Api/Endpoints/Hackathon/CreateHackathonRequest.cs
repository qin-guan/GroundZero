using FastEndpoints;

namespace GroundZero.Api.Endpoints.Hackathon;

public class CreateHackathonRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Venue { get; set; }
    public Uri HomepageUri { get; set; }
}
