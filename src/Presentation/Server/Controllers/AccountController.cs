namespace Server.Controllers;

public sealed class AccountController : ControllerBase
{
    #region Public Methods

    [HttpGet("~/api/account/login")]
    public ActionResult Login(string returnUrl)
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
        });
    }

    [ValidateAntiForgeryToken]
    [Authorize]
    [HttpPost("~/api/account/logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    #endregion Public Methods
}