using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed class LoginWith2faModel : PageModel
{
    #region Private Fields

    private readonly ILogger<LoginWith2faModel> _logger;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public LoginWith2faModel(
        SignInManager<ApplicationUserEntity> signInManager,
        UserManager<ApplicationUserEntity> userManager,
        ILogger<LoginWith2faModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Properties

    [BindProperty]
    public LoginWithTwoFactorAuthInput Input { get; set; } = default!;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        ApplicationUserEntity? user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        returnUrl ??= Url.Content("~/");

        ApplicationUserEntity? user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        string authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty) ?? string.Empty;

        Microsoft.AspNetCore.Identity.SignInResult? result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);
        string? userId = await _userManager.GetUserIdAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User with ID '{userId}' logged in with 2fa.", user.Id);
            return LocalRedirect(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
            return RedirectToPage("./Lockout");
        }
        else
        {
            _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }

    #endregion Public Methods
}