using FastEndpoints;
using SqlSugar;

namespace GroundZero.Web.Features.Hackathons.ListHackathons;

public class Endpoint(ISqlSugarClient db) : EndpointWithoutRequest<List<Response>>
{
    public override void Configure()
    {
        Get("/Hackathons");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var items = await db.Queryable<Entities.Hackathon>()
            .ToListAsync(ct);

        await SendOkAsync(items.ToResponse(), ct);
    }
}