using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class Disable2faModel : PageModel
{
    #region Private Fields

    private readonly ILogger<Disable2faModel> _logger;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public Disable2faModel(
        UserManager<ApplicationUserEntity> userManager,
        ILogger<Disable2faModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Properties

    [TempData]
    public string? StatusMessage { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGet()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
        {
            throw new InvalidOperationException($"Cannot disable 2FA for user as it's not currently enabled.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        IdentityResult disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred disabling 2FA.");
        }

        _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", _userManager.GetUserId(User));
        StatusMessage = "2fa has been disabled. You can re-enable 2fa when you setup an authenticator app";
        return RedirectToPage("./twoFactorAuthentication");
    }

    #endregion Public Methods
}