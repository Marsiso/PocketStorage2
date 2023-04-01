using Domain.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class ExternalLoginsModel : PageModel
{
    #region Private Fields

    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly IUserStore<ApplicationUserEntity> _userStore;

    #endregion Private Fields

    #region Public Constructors

    public ExternalLoginsModel(
        UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager,
        IUserStore<ApplicationUserEntity> userStore)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userStore = userStore;
    }

    #endregion Public Constructors

    #region Public Properties

    public IList<UserLoginInfo> CurrentLogins { get; set; } = default!;

    public IList<AuthenticationScheme> OtherLogins { get; set; } = default!;

    public bool ShowRemoveButton { get; set; }

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

        CurrentLogins = await _userManager.GetLoginsAsync(user);
        OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();

        string? passwordHash = default;
        if (_userStore is IUserPasswordStore<ApplicationUserEntity> userPasswordStore)
        {
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
        }

        ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;
        return Page();
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var userId = await _userManager.GetUserIdAsync(user);
        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync(userId) ?? throw new InvalidOperationException($"Unexpected error occurred loading external login info.");
        IdentityResult result = await _userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            StatusMessage = "The external login was not added. External logins can only be associated with one account.";
            return RedirectToPage();
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        StatusMessage = "The external login was added.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Request a redirect to the external login provider to link a login for the current user
        string? redirectUrl = Url.Page("./externalLogins", pageHandler: "LinkLoginCallback");
        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        IdentityResult result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            StatusMessage = "The external login was not removed.";
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "The external login was removed.";
        return RedirectToPage();
    }

    #endregion Public Methods
}