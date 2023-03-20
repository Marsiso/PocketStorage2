namespace Server.Controllers;

[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
    [HttpGet("Login")]
    public ActionResult Login(string returnUrl) => Challenge(new AuthenticationProperties { RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/" });

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout() => SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
}
