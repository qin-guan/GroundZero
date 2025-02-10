using GroundZero.Api.Entities;

namespace GroundZero.Api.Endpoints;

public static class HackathonEndpoints
{
    public static IResult GetHackathon()
    {
        return TypedResults.Ok();
    }
}