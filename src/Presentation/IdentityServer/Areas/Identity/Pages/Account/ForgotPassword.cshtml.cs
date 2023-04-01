using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed class ForgotPasswordModel : PageModel
{
    #region Private Fields

    private readonly IEmailSender _emailSender;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public ForgotPasswordModel(UserManager<ApplicationUserEntity> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    #endregion Public Constructors

    #region Public Properties

    [BindProperty]
    public ForgottenPasswordInput Input { get; set; } = default!;

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            ApplicationUserEntity? user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./forgotPasswordConfirmation");
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string callbackUrl = Url.Page(
                "/account/resetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme) ?? throw new NullReferenceException($"[{nameof(ExternalLoginModel)}] Null reference exception. Property: '{nameof(callbackUrl)}' Value: {null}");

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("./forgotPasswordConfirmation");
        }

        return Page();
    }

    #endregion Public Methods
}