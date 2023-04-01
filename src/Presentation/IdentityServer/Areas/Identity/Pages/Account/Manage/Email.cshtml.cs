using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class EmailModel : PageModel
{
    #region Private Fields

    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Private Methods

    private async Task LoadAsync(ApplicationUserEntity user)
    {
        string? email = await _userManager.GetEmailAsync(user);

        email ??= string.Empty;
        Email = email;

        Input = new SetEmailInput
        {
            NewEmail = email,
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    #endregion Private Methods

    #region Public Constructors

    public EmailModel(
            UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    #endregion Public Constructors

    #region Public Properties

    public string Email { get; set; } = default!;

    [BindProperty]
    public SetEmailInput Input { get; set; } = default!;

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        string? email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            string userId = await _userManager.GetUserIdAsync(user);
            string code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string? callbackUrl = Url.Page(
                "/account/confirmEmailChange",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                protocol: Request.Scheme) ?? throw new NullReferenceException($"Null reference exception. Variable: '{nameof(callbackUrl)}' Value: '{null}'");
            await _emailSender.SendEmailAsync(
                Input.NewEmail,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Confirmation link to change email sent. Please check your email.";
            return RedirectToPage();
        }

        StatusMessage = "Your email is unchanged.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        string userId = await _userManager.GetUserIdAsync(user);
        string? email = await _userManager.GetEmailAsync(user) ?? throw new NullReferenceException($"Null reference exception. Variable: '{nameof(email)}' Value: '{null}'");
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string? callbackUrl = Url.Page(
            "/account/confirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = userId, code = code },
            protocol: Request.Scheme) ?? throw new NullReferenceException($"Null reference exception. Variable: '{nameof(callbackUrl)}' Value: '{null}'");
        await _emailSender.SendEmailAsync(
            email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        StatusMessage = "Verification email sent. Please check your email.";
        return RedirectToPage();
    }

    #endregion Public Methods
}