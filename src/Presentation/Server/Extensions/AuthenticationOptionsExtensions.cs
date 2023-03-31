namespace Server.Extensions;

public static class AuthenticationOptionsExtensions
{
    #region Public Methods

    public static void Configure(this AuthenticationOptions options)
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }

    #endregion Public Methods
}