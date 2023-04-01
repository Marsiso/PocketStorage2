using Domain.Constants;
using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ExternalLoginModel : PageModel
{
    #region Private Fields

    private readonly IEmailSender _emailSender;
    private readonly IUserEmailStore<ApplicationUserEntity> _emailStore;
    private readonly ILogger<ExternalLoginModel> _logger;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly IUserStore<ApplicationUserEntity> _userStore;

    #endregion Private Fields

    #region Private Methods

    private ApplicationUserEntity CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUserEntity>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUserEntity)}'. " +
                $"Ensure that '{nameof(ApplicationUserEntity)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
        }
    }

    private IUserEmailStore<ApplicationUserEntity> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUserEntity>)_userStore;
    }

    #endregion Private Methods

    #region Public Constructors

    public ExternalLoginModel(
                                    SignInManager<ApplicationUserEntity> signInManager,
            UserManager<ApplicationUserEntity> userManager,
            IUserStore<ApplicationUserEntity> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _logger = logger;
        _emailSender = emailSender;
    }

    #endregion Public Constructors

    #region Public Properties

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public ExternalSigninInput Input { get; set; } = default!;

    public string ProviderDisplayName { get; set; } = default!;

    public string ReturnUrl { get; set; } = default!;

    #endregion Public Properties

    #region Public Methods

    public IActionResult OnGet() => RedirectToPage("./login");

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./login", new { ReturnUrl = returnUrl });
        }
        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./login", new { ReturnUrl = returnUrl });
        }

        // Sign in the user with this external login provider if the user already has a login.
        Microsoft.AspNetCore.Identity.SignInResult result =
            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal?.Identity?.Name, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToPage("./lockout");
        }
        else
        {
            // If the user does not have an account, then ask the user to create an account.
            Input = new();
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName ?? throw new NullReferenceException($"[{nameof(ExternalLoginModel)}] Null reference exception. Property: '{nameof(ProviderDisplayName)}' Value: {ProviderDisplayName}");
            if (info.Principal.HasClaim(claim => claim.Type == ClaimTypes.Email))
            {
                Input.Email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            }

            if (info.Principal.HasClaim(claim => claim.Type == ClaimTypes.GivenName))
            {
                Input.GivenName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
            }

            if (info.Principal.HasClaim(claim => claim.Type == ClaimTypes.Surname))
            {
                Input.FamilyName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
            }

            return Page();
        }
    }

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        // Request a redirect to the external login provider.
        string? redirectUrl = Url.Page("./externalLogin", pageHandler: "Callback", values: new { returnUrl });
        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        // Get the information about the user from the external login provider
        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./login", new { ReturnUrl = returnUrl });
        }

        if (ModelState.IsValid)
        {
            ApplicationUserEntity user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            user.GivenName = Input.GivenName;
            user.OtherName = Input.OtherName;
            user.FamilyName = Input.FamilyName;

            IdentityResult result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account without password.");

                result = await _userManager.AddToRoleAsync(user, ApplicationConstants.Roles.DefaultAccess);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Newly created user added to default access role.");

                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        string callbackUrl = Url.Page(
                            "/account/confirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme) ?? throw new NullReferenceException($"[{nameof(ExternalLoginModel)}] Null reference exception. Property: '{nameof(callbackUrl)}' Value: {null}");

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't
                        // have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./registerConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
        ReturnUrl = returnUrl;

        return Page();
    }

    #endregion Public Methods
}