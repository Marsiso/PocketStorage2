namespace Server.Controllers;

public sealed class AccountController : ControllerBase
{
    [HttpGet("~/api/account/login")]
    public ActionResult Login(string returnUrl) => Challenge(new AuthenticationProperties { RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/" });

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpPost("~/api/account/logout")]
    public IActionResult Logout() => SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
}
