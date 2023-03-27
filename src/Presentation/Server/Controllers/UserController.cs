using Domain.Identity.Entities;

namespace Server.Controllers;

// original source https://github.com/berhir/BlazorWebAssemblyCookieAuth
[ApiController]
public sealed class UserController : ControllerBase
{
    [HttpGet("~/api/user")]
    [AllowAnonymous]
    public IActionResult GetCurrentUser()
    {
        if (!User?.Identity?.IsAuthenticated ?? true)
        {
            return Ok(ApplicationUserInfo.Anonymous);
        }

        var userInfo = new ApplicationUserInfo { IsAuthenticated = true };

        if (User?.Identity is ClaimsIdentity claimsIdentity)
        {
            userInfo.NameClaimType = claimsIdentity.NameClaimType;
            userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = ClaimTypes.Name;
            userInfo.RoleClaimType = ClaimTypes.Role;
        }

        if (User?.Claims?.Any() ?? false)
        {
            IList<ApplicationClaimValue> claims = new List<ApplicationClaimValue>();
            foreach (var claim in User.Claims)
            {
                if (claim.Type.Equals(userInfo.NameClaimType, StringComparison.OrdinalIgnoreCase))
                {
                    claims.Add(new ApplicationClaimValue(userInfo.NameClaimType, claim.Value));
                }
                else if (claim.Type.Equals(userInfo.RoleClaimType, StringComparison.OrdinalIgnoreCase))
                {
                    claims.Add(new ApplicationClaimValue(userInfo.RoleClaimType, claim.Value));
                }
            }

            userInfo.Claims = claims;
        }

        return Ok(userInfo);
    }
}
