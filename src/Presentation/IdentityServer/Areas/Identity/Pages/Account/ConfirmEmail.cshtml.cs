using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed class ConfirmEmailModel : PageModel
{
    #region Private Fields

    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public ConfirmEmailModel(UserManager<ApplicationUserEntity> userManager)
    {
        _userManager = userManager;
    }

    #endregion Public Constructors

    #region Public Properties

    [TempData]
    public string? StatusMessage { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync(string? userId, string? code)
    {
        if (userId == null || code == null)
        {
            return RedirectToPage("/index");
        }

        ApplicationUserEntity? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
        StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
        return Page();
    }

    #endregion Public Methods
}