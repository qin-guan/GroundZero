using System.Security.Claims;

namespace GroundZero.Web.Authentication;

public static class AuthenticationExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var raw = principal.Identities
            .Single(i => i.Name == ClaimsIdentity.DefaultIssuer).Claims
            .Single(c => c.Type == ClaimTypes.NameIdentifier);

        return Guid.Parse(raw.Value);
    }
}