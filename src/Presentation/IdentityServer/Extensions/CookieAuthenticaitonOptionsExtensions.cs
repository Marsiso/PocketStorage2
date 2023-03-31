using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdentityServer.Extensions;

public static class CookieAuthenticaitonOptionsExtensions
{
    #region Public Methods

    public static void Configure(this CookieAuthenticationOptions options)
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);

        options.LoginPath = "/identity/account/login";
        options.AccessDeniedPath = "/identity/account/accessDenied";
        options.SlidingExpiration = true;
    }

    #endregion Public Methods
}