using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.Extensions;

public static class AuthenticationExtensions
{
    #region Public Methods

    public static AuthenticationBuilder AddExternalProviders(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        return builder.AddGoogle(options =>
        {
            options.ClientId = configuration["Clients:Google:Id"] ?? string.Empty;
            options.ClientSecret = configuration["Clients:Google:Secret"] ?? string.Empty;
        })
        .AddFacebook(options =>
        {
            options.ClientId = configuration["Clients:Facebook:Id"] ?? string.Empty;
            options.ClientSecret = configuration["Clients:Facebook:Secret"] ?? string.Empty;
        })
        .AddMicrosoftAccount(options =>
        {
            options.ClientId = configuration["Clients:MicrosoftAccount:Id"] ?? string.Empty;
            options.ClientSecret = configuration["Clients:MicrosoftAccount:Secret"] ?? string.Empty;
        });
    }

    #endregion Public Methods
}