namespace Server.Extensions;

public static class AuthenticationExtensions
{
    #region Public Methods

    public static AuthenticationBuilder AddApplicationAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        });
    }

    #endregion Public Methods
}