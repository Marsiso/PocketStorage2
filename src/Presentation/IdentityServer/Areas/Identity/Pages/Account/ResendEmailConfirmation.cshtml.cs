using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ResendEmailConfirmationModel : PageModel
{
    #region Private Fields

    private readonly IEmailSender _emailSender;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public ResendEmailConfirmationModel(UserManager<ApplicationUserEntity> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to
    /// be used directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public ResetPasswordInput Input { get; set; } = default!;

    #endregion Public Properties

    #region Public Methods

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApplicationUserEntity? user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }

        string userId = await _userManager.GetUserIdAsync(user);
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = Url.Page(
            "/account/confirmEmail",
            pageHandler: null,
            values: new { userId = userId, code = code },
            protocol: Request.Scheme) ?? throw new NullReferenceException($"Null reference exception. Property: {nameof(callbackUrl)} Value: {null}");

        await _emailSender.SendEmailAsync(
            Input.Email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");

        return Page();
    }

    #endregion Public Methods
}