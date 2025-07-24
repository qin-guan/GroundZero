using Riok.Mapperly.Abstractions;

namespace GroundZero.Web.Features.Hackathons.CreateHackathon;

public class Request
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Venue { get; set; }

    public string HomepageUri { get; set; }
}

public class Response
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Venue { get; set; }

    public string HomepageUri { get; set; }
}

[Mapper]
public static partial class Mapper
{
    public static partial Entities.Hackathon ToEntity(this Request req);
    public static partial Response ToResponse(this Entities.Hackathon hackathon);
}