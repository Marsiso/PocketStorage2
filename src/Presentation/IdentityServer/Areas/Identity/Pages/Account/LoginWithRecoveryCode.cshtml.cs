using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed class LoginWithRecoveryCodeModel : PageModel
{
    #region Private Fields

    private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public LoginWithRecoveryCodeModel(
        SignInManager<ApplicationUserEntity> signInManager,
        UserManager<ApplicationUserEntity> userManager,
        ILogger<LoginWithRecoveryCodeModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Properties

    [BindProperty]
    public LoginWithRecoveryCodeInput Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        // Ensure the user has gone through the user name & password screen first
        ApplicationUserEntity? user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApplicationUserEntity? user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        string recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty) ?? string.Empty;

        Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        string userId = await _userManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            _logger.LogInformation($"User with ID '{userId}' logged in with a recovery code.", user.Id);
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToPage("./lockout");
        }
        else
        {
            _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            return Page();
        }
    }

    #endregion Public Methods
}