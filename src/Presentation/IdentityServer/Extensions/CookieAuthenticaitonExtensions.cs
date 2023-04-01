using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdentityServer.Extensions;

public static class CookieAuthenticaitonExtensions
{
    #region Public Methods

    public static CookieAuthenticationOptions ConfigureApplicationCookie(this CookieAuthenticationOptions options)
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);

        options.LoginPath = "/identity/account/login";
        options.AccessDeniedPath = "/identity/account/accessDenied";
        options.SlidingExpiration = true;

        return options;
    }

    #endregion Public Methods
}