using FastEndpoints;
using Groundzero.Api.Mappers;
using GroundZero.Api.Context;
using GroundZero.Api.Dtos;
using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GroundZero.Api.Endpoints.Hackathon;

public class ListHackathonEndpoint(UserManager<AppUser> userManager, AppDbContext dbContext) : EndpointWithoutRequest<IEnumerable<HackathonResponse>>
{
  public override void Configure()
  {
    Get("/Hackathon");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userId = userManager.GetUserId(User);
    ArgumentNullException.ThrowIfNull(userId);

    var user = await dbContext.Users
      .Include(u => u.Hackathons)
        .ThenInclude(h => h.Hackathon)
      .Where(u => u.Id == Guid.Parse(userId))
      .Select(u => u.Hackathons.Select(h => h.Hackathon))
      .SingleOrDefaultAsync(ct);

    ArgumentNullException.ThrowIfNull(user);

    await SendOkAsync(
      user.ToResponse(),
      ct
    );
  }
}
