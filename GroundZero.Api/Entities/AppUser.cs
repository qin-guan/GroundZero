using Microsoft.AspNetCore.Identity;

namespace GroundZero.Api.Entities;

public class AppUser : IdentityUser<Guid>
{
    public ICollection<Participant> HackathonsParticipatedIn { get; set; } = [];
    public ICollection<Judge> HackathonsJudgedIn { get; set; } = [];
    public ICollection<Hackathon> HackathonsAdminIn { get; set; } = [];
}
