using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class GenerateRecoveryCodesModel : PageModel
{
    #region Private Fields

    private readonly ILogger<GenerateRecoveryCodesModel> _logger;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public GenerateRecoveryCodesModel(
        UserManager<ApplicationUserEntity> userManager,
        ILogger<GenerateRecoveryCodesModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Properties

    [TempData]
    public string[]? RecoveryCodes { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        bool isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException($"Cannot generate recovery codes for user because they do not have 2FA enabled.");
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

        bool isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        string userId = await _userManager.GetUserIdAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException($"Cannot generate recovery codes for user as they do not have 2FA enabled.");
        }

        IEnumerable<string>? recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        RecoveryCodes = recoveryCodes?.ToArray();

        _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
        StatusMessage = "You have generated new recovery codes.";
        return RedirectToPage("./showRecoveryCodes");
    }

    #endregion Public Methods
}