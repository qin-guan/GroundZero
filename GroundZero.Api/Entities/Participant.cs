using System.Security.Cryptography;
using GroundZero.Gavel;

namespace GroundZero.Api.Entities;

public class Participant
{
    public Guid Id { get; set; }
    
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }
    
    public Guid UserId { get; set; }
    public AppUser User { get; set; }
}