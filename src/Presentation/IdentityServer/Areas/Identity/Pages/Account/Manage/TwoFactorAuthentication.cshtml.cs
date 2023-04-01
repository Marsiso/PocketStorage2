using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class TwoFactorAuthenticationModel : PageModel
{
    #region Private Fields

    private readonly ILogger<TwoFactorAuthenticationModel> _logger;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public TwoFactorAuthenticationModel(
        UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager,
        ILogger<TwoFactorAuthenticationModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Properties

    public bool HasAuthenticator { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    public int RecoveryCodesLeft { get; set; }

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

        HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
        Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
        RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _signInManager.ForgetTwoFactorClientAsync();
        StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return RedirectToPage();
    }

    #endregion Public Methods
}