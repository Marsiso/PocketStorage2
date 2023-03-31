using Microsoft.AspNetCore.Antiforgery;

namespace Server.Extensions;

public static class AntiforgeryServiceOptionsExtensions
{
    #region Public Methods

    public static void Configure(this AntiforgeryOptions options)
    {
        options.HeaderName = AntiforgeryDefaults.HeaderName;
        options.Cookie.Name = AntiforgeryDefaults.CookieName;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }

    #endregion Public Methods
}