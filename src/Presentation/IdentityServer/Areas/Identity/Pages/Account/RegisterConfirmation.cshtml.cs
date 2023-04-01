using Domain.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityServer.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class RegisterConfirmationModel : PageModel
{
    #region Private Fields

    private readonly IEmailSender _sender;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public RegisterConfirmationModel(UserManager<ApplicationUserEntity> userManager, IEmailSender sender)
    {
        _userManager = userManager;
        _sender = sender;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to
    /// be used directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public bool DisplayConfirmAccountLink { get; set; } = default!;

    /// <summary>
    /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to
    /// be used directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to
    /// be used directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string EmailConfirmationUrl { get; set; } = default!;

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync(string email, string? returnUrl = null)
    {
        if (email == null)
        {
            return RedirectToPage("/index");
        }

        returnUrl ??= Url.Content("~/");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"Unable to load user with email '{email}'.");
        }

        Email = email;
        // Once you add a real email sender, you should remove this code that lets you confirm the account
        DisplayConfirmAccountLink = true;
        if (DisplayConfirmAccountLink)
        {
            string userId = await _userManager.GetUserIdAsync(user);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            EmailConfirmationUrl = Url.Page(
                "/account/confirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme) ?? throw new NullReferenceException($"Null reference exception. Property: {nameof(EmailConfirmationUrl)} Value: {EmailConfirmationUrl}");
        }

        return Page();
    }

    #endregion Public Methods
}