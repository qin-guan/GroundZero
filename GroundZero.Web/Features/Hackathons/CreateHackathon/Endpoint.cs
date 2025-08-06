using FastEndpoints;
using GroundZero.Web.Authentication;
using GroundZero.Web.Entities;
using SqlSugar;

namespace GroundZero.Web.Features.Hackathons.CreateHackathon;

public class Endpoint(ISqlSugarClient db) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/Hackathons");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = User.GetUserId();

        var hackathon = req.ToEntity();
        hackathon.Organizers =
        [
            new Organizer
            {
                UserId = userId,
                Type = OrganizerType.Admin
            }
        ];

        var entity = await db.InsertNav(hackathon).Include(h => h.Organizers).ExecuteReturnEntityAsync();

        await Send.OkAsync(entity.ToResponse(), ct);
    }
}