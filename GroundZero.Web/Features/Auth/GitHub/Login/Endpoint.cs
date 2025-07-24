using FastEndpoints;
using Microsoft.AspNetCore.Authentication;

namespace GroundZero.Web.Features.Auth.GitHub.Login;

public class Endpoint: EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/Auth/GitHub/Login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await HttpContext.ChallengeAsync(new AuthenticationProperties
        {
            RedirectUri = "/Auth/GitHub/Callback"
        });
    }
}