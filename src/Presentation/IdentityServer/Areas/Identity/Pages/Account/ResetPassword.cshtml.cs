using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed class ResetPasswordModel : PageModel
{
    #region Private Fields

    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public ResetPasswordModel(UserManager<ApplicationUserEntity> userManager)
    {
        _userManager = userManager;
    }

    #endregion Public Constructors

    #region Public Properties

    [BindProperty]
    public ResetPasswordInput Input { get; set; } = default!;

    #endregion Public Properties

    #region Public Methods

    public IActionResult OnGet(string? code = null)
    {
        if (code == null)
        {
            return BadRequest("A code must be supplied for password reset.");
        }
        else
        {
            Input = new ResetPasswordInput
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };
            return Page();
        }
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
            // Don't reveal that the user does not exist
            return RedirectToPage("./resetPasswordConfirmation");
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            return RedirectToPage("./resetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }

    #endregion Public Methods
}