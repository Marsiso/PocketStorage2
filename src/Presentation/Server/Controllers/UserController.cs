using Domain.Identity.Entities;

namespace Server.Controllers;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
[Route("api/[controller]")]
[ApiController]
public sealed class UserController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser() => Ok(CreateUserInfo(User));

    private ApplicationUserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (!claimsPrincipal?.Identity?.IsAuthenticated ?? true)
        {
            return ApplicationUserInfo.Anonymous;
        }

        var userInfo = new ApplicationUserInfo { IsAuthenticated = true };

        if (claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
        {
            userInfo.NameClaimType = claimsIdentity.NameClaimType;
            userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = ClaimTypes.Name;
            userInfo.RoleClaimType = ClaimTypes.Role;
        }

        if (claimsPrincipal?.Claims?.Any() ?? false)
        {
            // Add just the name claim
            var claims = claimsPrincipal
                .FindAll(userInfo.NameClaimType)
                .Select(claim => new ApplicationClaimValue(userInfo.NameClaimType, claim.Value))
                .ToList();

            // Uncomment this code if you want to send additional claims to the client.
            //var claims = claimsPrincipal.Claims.Select(u => new ClaimValue(u.Type, u.Value)).ToList();

            userInfo.Claims = claims;
        }

        return userInfo;
    }
}
