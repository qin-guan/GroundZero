using Microsoft.AspNetCore.Identity;

namespace GroundZero.Api.Entities;

public class AppUser : IdentityUser<Guid>
{
    public ICollection<HackathonUser> Hackathons { get; set; } = [];
}
