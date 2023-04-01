using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class PersonalDataModel : PageModel
{
    #region Private Fields

    private readonly ILogger<PersonalDataModel> _logger;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public PersonalDataModel(
        UserManager<ApplicationUserEntity> userManager,
        ILogger<PersonalDataModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task<IActionResult> OnGet()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        return Page();
    }

    #endregion Public Methods
}