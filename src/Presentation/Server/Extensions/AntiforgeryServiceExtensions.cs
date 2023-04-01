namespace Server.Extensions;

public static class AntiforgeryServiceExtensions
{
    #region Public Methods

    public static IServiceCollection AddApplicationAntiforgery(this IServiceCollection services)
    {
        return services.AddAntiforgery(options =>
        {
            options.HeaderName = AntiforgeryDefaults.HeaderName;
            options.Cookie.Name = AntiforgeryDefaults.CookieName;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    #endregion Public Methods
}