using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using System.Text.Json;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public sealed class DownloadPersonalDataModel : PageModel
{
    #region Private Fields

    private readonly ILogger<DownloadPersonalDataModel> _logger;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public DownloadPersonalDataModel(
        UserManager<ApplicationUserEntity> userManager,
        ILogger<DownloadPersonalDataModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public IActionResult OnGet()
    {
        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUserEntity? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

        // Only include personal data for download
        Dictionary<string, string> personalData = new Dictionary<string, string>();
        IEnumerable<PropertyInfo> personalDataProps = typeof(ApplicationUserEntity)
            .GetProperties()
            .Where(propertyInfo => Attribute.IsDefined(propertyInfo, typeof(PersonalDataAttribute)));
        foreach (var propertyInfo in personalDataProps)
        {
            personalData.Add(propertyInfo.Name, propertyInfo.GetValue(user)?.ToString() ?? "null");
        }

        IList<UserLoginInfo>? logins = await _userManager.GetLoginsAsync(user);
        foreach (var login in logins)
        {
            personalData.Add($"{login.LoginProvider} external login provider key", login.ProviderKey);
        }

        personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user)
            ?? throw new NullReferenceException($"Null reference exception. Variable: 'personalData[Authenticator Key]' Value: '{null}'"));

        Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }

    #endregion Public Methods
}