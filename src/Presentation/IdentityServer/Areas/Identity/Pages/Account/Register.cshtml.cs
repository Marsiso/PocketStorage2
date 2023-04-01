using Domain.Constants;
using Domain.Data.Entities;
using IdentityServer.Data.Dtos.Post;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Areas.Identity.Pages.Account;

public sealed partial class RegisterModel : PageModel
{
    #region Private Fields

    private readonly IEmailSender _emailSender;
    private readonly IUserEmailStore<ApplicationUserEntity> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
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
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
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

    public RegisterModel(
                UserManager<ApplicationUserEntity> userManager,
        IUserStore<ApplicationUserEntity> userStore,
        SignInManager<ApplicationUserEntity> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    #endregion Public Constructors

    #region Public Properties

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = default!;

    [BindProperty]
    public SigninInput Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    #endregion Public Properties

    #region Public Methods

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (ModelState.IsValid)
        {
            ApplicationUserEntity user = CreateUser();
            user.GivenName = Input.GivenName;
            user.OtherName = Input.OtherName;
            user.FamilyName = Input.FamilyName;

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            IdentityResult result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                result = await _userManager.AddToRoleAsync(user, ApplicationConstants.Roles.DefaultAccess);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Newly created user added to default access role.");

                    string userId = await _userManager.GetUserIdAsync(user);
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    string callbackUrl = Url.Page(
                        "/account/confirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme) ?? throw new NullReferenceException($"Null reference exception. Property: {nameof(callbackUrl)} Value: {null}");

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }

    #endregion Public Methods
}