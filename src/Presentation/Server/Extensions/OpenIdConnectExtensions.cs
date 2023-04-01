﻿using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace Server.Extensions;

public static class OpenIdConnectExtensions
{
    #region Public Methods

    public static AuthenticationBuilder AddApplicationOpenIdConnect(this AuthenticationBuilder services, IConfiguration configuration)
    {
        return services
            .AddOpenIdConnect(options =>
            {
                string authority = configuration["OpenIdConnect:Authority"] ?? string.Empty;
                string clientId = configuration["OpenIdConnect:ClientId"] ?? string.Empty;
                string clientSecret = configuration["OpenIdConnect:ClientSecret"] ?? string.Empty;

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
            });
    }

    #endregion Public Methods
}