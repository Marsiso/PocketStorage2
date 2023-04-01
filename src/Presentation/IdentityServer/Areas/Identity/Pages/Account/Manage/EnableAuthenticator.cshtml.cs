using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class EnableAuthenticatorModel : PageModel
{
    #region Private Fields

    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private readonly ILogger<EnableAuthenticatorModel> _logger;
    private readonly UrlEncoder _urlEncoder;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Private Methods

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUserEntity user)
    {
        // Load the authenticator key & QR code URI to display on the form
        string? unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        unformattedKey ??= string.Empty;
        SharedKey = FormatKey(unformattedKey);

        string? email = await _userManager.GetEmailAsync(user);
        email ??= string.Empty;
        AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
    }

    #endregion Private Methods

    #region Public Constructors

    public EnableAuthenticatorModel(
                    UserManager<ApplicationUserEntity> userManager,
        ILogger<EnableAuthenticatorModel> logger,
        UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _logger = logger;
        _urlEncoder = urlEncoder;
    }

    #endregion Public Constructors

    #region Public Properties

    public string AuthenticatorUri { get; set; } = default!;

    [BindProperty]
    public EnableAuthenticatorInput Input { get; set; } = default!;

    [TempData]
    public string[]? RecoveryCodes { get; set; }

    public string SharedKey { get; set; } = default!;

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

        await LoadSharedKeyAndQrCodeUriAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return Page();
        }

        // Strip spaces and hyphens
        string verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        bool is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError("Input.Code", "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return Page();
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        var userId = await _userManager.GetUserIdAsync(user);
        _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        StatusMessage = "Your authenticator app has been verified.";

        if (await _userManager.CountRecoveryCodesAsync(user) == 0)
        {
            IEnumerable<string>? recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            RecoveryCodes = recoveryCodes?.ToArray();
            return RedirectToPage("./showRecoveryCodes");
        }
        else
        {
            return RedirectToPage("./twoFactorAuthentication");
        }
    }

    #endregion Public Methods
}