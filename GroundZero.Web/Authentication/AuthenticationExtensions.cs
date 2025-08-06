using System.Security.Claims;

namespace GroundZero.Web.Authentication;

public static class AuthenticationExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var raw = principal.Claims
            .Single(c => c.Type == ClaimTypes.NameIdentifier && c.Issuer == ClaimsIdentity.DefaultIssuer);

        return Guid.Parse(raw.Value);
    }
}