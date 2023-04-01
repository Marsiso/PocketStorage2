using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class ShowRecoveryCodesModel : PageModel
{
    #region Public Properties

    [TempData]
    public string[]? RecoveryCodes { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    #endregion Public Properties

    #region Public Methods

    public IActionResult OnGet()
    {
        if (RecoveryCodes == null || RecoveryCodes.Length == 0)
        {
            return RedirectToPage("./twoFactorAuthentication");
        }

        return Page();
    }

    #endregion Public Methods
}