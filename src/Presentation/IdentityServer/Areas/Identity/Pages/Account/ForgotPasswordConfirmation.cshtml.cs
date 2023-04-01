using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ForgotPasswordConfirmation : PageModel
{
    #region Public Methods

    public void OnGet()
    {
    }

    #endregion Public Methods
}