
using GroundZero.Api.Dtos;
using GroundZero.Api.Entities;
using Riok.Mapperly.Abstractions;

namespace Groundzero.Api.Mappers;

[Mapper]
public static partial class HackathonMapper
{
    public static partial HackathonResponse ToResponse(this Hackathon hackathon);
    public static partial IEnumerable<HackathonResponse> ToResponse(this IEnumerable<Hackathon> hackathons);
}
