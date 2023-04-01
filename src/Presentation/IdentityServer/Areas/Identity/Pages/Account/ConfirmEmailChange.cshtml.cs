using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed class ConfirmEmailChangeModel : PageModel
{
    #region Private Fields

    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public ConfirmEmailChangeModel(UserManager<ApplicationUserEntity> userManager, SignInManager<ApplicationUserEntity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to
    /// be used directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string? StatusMessage { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync(string? userId, string? email, string? code)
    {
        if (userId == null || email == null || code == null)
        {
            return RedirectToPage("/index");
        }

        ApplicationUserEntity? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult result = await _userManager.ChangeEmailAsync(user, email, code);
        if (!result.Succeeded)
        {
            StatusMessage = "Error changing email.";
            return Page();
        }

        // In our UI email and user name are one and the same, so when we update the email we need
        // to update the user name.
        IdentityResult setUserNameResult = await _userManager.SetUserNameAsync(user, email);
        if (!setUserNameResult.Succeeded)
        {
            StatusMessage = "Error changing user name.";
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Thank you for confirming your email change.";

        return Page();
    }

    #endregion Public Methods
}