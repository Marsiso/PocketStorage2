using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class IndexModel : PageModel
{
    #region Private Fields

    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Private Methods

    private async Task LoadAsync(ApplicationUserEntity user)
    {
        string? userName = await _userManager.GetUserNameAsync(user);
        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Username = userName;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber
        };
    }

    #endregion Private Methods

    #region Public Constructors

    public IndexModel(
            UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #endregion Public Constructors

    #region Public Properties

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public string? Username { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
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
            await LoadAsync(user);
            return Page();
        }

        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            IdentityResult setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    #endregion Public Methods

    #region Public Classes

    public sealed class InputModel
    {
        #region Public Properties

        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        #endregion Public Properties
    }

    #endregion Public Classes
}