using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Data.Seed;

public sealed class Worker : IHostedService
{
    #region Private Fields

    private readonly IServiceProvider _serviceProvider;

    #endregion Private Fields

    #region Public Constructors

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        #region BlazorWasm

        if (await manager.FindByClientIdAsync("BlazorWasmClient") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "BlazorWasmClient",
                ClientSecret = "BlazorWasmClient-Secret",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Blazor WebAssembly Application",
                RedirectUris = { new Uri("https://localhost:5001/signin-oidc"), new Uri("https://localhost:44132/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("https://localhost:5001/signout-callback-oidc"), new Uri("https://localhost:44132/signout-callback-oidc") },
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
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "offline_access"
                },
                Requirements = { Requirements.Features.ProofKeyForCodeExchange }
            });
        }

        #endregion BlazorWasm
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion Public Methods
}