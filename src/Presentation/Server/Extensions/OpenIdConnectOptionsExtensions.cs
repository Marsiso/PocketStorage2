using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace Server.Extensions;

public static class OpenIdConnectOptionsExtensions
{
    #region Public Methods

    public static void Configure(this OpenIdConnectOptions options, string authority, string clientId, string clientSecret)
    {
        if (string.IsNullOrEmpty(authority))
        {
            throw new ArgumentNullException(nameof(authority),
                $"OpenID Connect authority cannot be a null or empty string. Parameter name: {nameof(authority)} Value: {authority}");
        }

        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentNullException(nameof(clientId),
                $"OpenID Connect client ID cannot be a null or empty string. Parameter name: {nameof(clientId)} Value: {clientId}");
        }

        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentNullException(nameof(clientSecret),
                $"OpenID Connect client secret cannot be a null or empty string. Parameter name: {nameof(clientSecret)} Value: {clientSecret}");
        }

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = authority;
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.RequireHttpsMetadata = true;
        options.ResponseType = OpenIddictConstants.ResponseTypes.Code;
        options.UsePkce = true;
        options.Scope.Add(OpenIddictConstants.Scopes.Profile);
        options.Scope.Add(OpenIddictConstants.Scopes.Email);
        options.Scope.Add(OpenIddictConstants.Scopes.Roles);
        options.Scope.Add(OpenIddictConstants.Scopes.OfflineAccess);
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RefreshInterval = TimeSpan.FromMinutes(3);
        options.AutomaticRefreshInterval = TimeSpan.FromMinutes(10);
        options.AccessDeniedPath = "/";
        options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = OpenIddictConstants.Claims.Name };
    }

    #endregion Public Methods
}