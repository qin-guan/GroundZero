using Riok.Mapperly.Abstractions;

namespace GroundZero.Web.Features.Hackathons.ListHackathons;

public class Response
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Venue { get; set; } = null!;

    public string HomepageUri { get; set; } = null!;
}

[Mapper]
public static partial class Mapper
{
    public static partial List<Response> ToResponse(this List<Entities.Hackathon> hackathons);
}