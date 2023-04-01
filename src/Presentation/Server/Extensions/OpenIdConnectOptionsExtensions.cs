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
            string errorMessage = $"OpenID Connect authority cannot be a null or empty string. Parameter name: {nameof(authority)} Value: {authority}";
            throw new ArgumentNullException(nameof(authority), errorMessage);
        }

        if (string.IsNullOrEmpty(clientId))
        {
            string errorMessage = $"OpenID Connect client ID cannot be a null or empty string. Parameter name: {nameof(clientId)} Value: {clientId}";
            throw new ArgumentNullException(nameof(clientId), errorMessage);
        }

        if (string.IsNullOrEmpty(clientSecret))
        {
            string errorMessage = $"OpenID Connect client secret cannot be a null or empty string. Parameter name: {nameof(clientSecret)} Value: {clientSecret}";
            throw new ArgumentNullException(nameof(clientSecret), errorMessage);
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