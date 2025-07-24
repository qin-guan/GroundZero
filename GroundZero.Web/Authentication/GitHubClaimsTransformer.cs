using System.Security.Claims;
using GroundZero.Web.Entities;
using Microsoft.AspNetCore.Authentication;
using SqlSugar;

namespace GroundZero.Web.Authentication;

public class GitHubClaimsTransformer(ISqlSugarClient db) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userName = principal.Claims.First(c => c.Type == ClaimTypes.Name)?.Value ?? throw new Exception();
        var account = await db.Queryable<GitHubOnlineAccount>()
            .Includes(a => a.User)
            .SingleAsync(a => a.UserName == userName);

        if (account is null)
        {
            account = await db.InsertNav(new GitHubOnlineAccount
                {
                    UserName = userName,
                    User = new User
                    {
                        Name = userName
                    }
                })
                .Include(g => g.User)
                .ExecuteReturnEntityAsync();
        }

        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, ClaimsIdentity.DefaultIssuer)
        ]);
        principal.AddIdentity(identity);

        return principal;
    }
}