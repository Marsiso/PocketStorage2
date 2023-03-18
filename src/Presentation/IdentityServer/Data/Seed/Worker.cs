using OpenIddict.Abstractions;
using System.Globalization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Data.Seed;

public sealed class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        #region Postman
        string postmanClient = "PostManClient";
        if (await manager.FindByClientIdAsync(postmanClient) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = postmanClient,
                ClientSecret = "PostMan-Secret",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Postman UI Application",
                RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Prefixes.Scope + "apiBff"
                },
                Requirements = { Requirements.Features.ProofKeyForCodeExchange }
            });
        }
        #endregion

        #region Resource_Bff
        string resourceBff = "Resource_Bff";
        if (await manager.FindByClientIdAsync(resourceBff) == null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = resourceBff,
                ClientSecret = "Resource-Bff-Secret",
                Permissions = { Permissions.Endpoints.Introspection }
            };

            await manager.CreateAsync(descriptor);
        }

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        if (await scopeManager.FindByNameAsync("apiBff") == null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                DisplayName = "BFF API Access",
                DisplayNames = { [CultureInfo.GetCultureInfo("en-US")] = "BFF APP" },
                Name = "apiBff",
                Resources = { resourceBff }
            });
        }
        #endregion
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
