using FastEndpoints;
using Groundzero.Api.Mappers;
using GroundZero.Api.Context;
using GroundZero.Api.Dtos;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace GroundZero.Api.Endpoints.Hackathon;

public class CreateHackathonEndpoint(UserManager<AppUser> userManager, AppDbContext dbContext) : Endpoint<CreateHackathonRequest, HackathonResponse>
{
    public override void Configure()
    {
        Post("/Hackathon");
    }

    public override async Task HandleAsync(CreateHackathonRequest req, CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        ArgumentNullException.ThrowIfNull(user);

        var entity = await dbContext.Hackathons.AddAsync(new Entities.Hackathon
        {
            Name = req.Name,
            Description = req.Description,
            Venue = req.Venue,
            HomepageUri = req.HomepageUri,
            Organizers = [
            new Organizer
        {
          OrganizerType =OrganizerType.Admin,
          UserId = user.Id,
        }
          ]
        }, ct);

        await SendOkAsync(
          entity.Entity.ToResponse(),
          ct
        );
    }
}
