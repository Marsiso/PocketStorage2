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
        using IServiceScope scope = _serviceProvider.CreateScope();
        IOpenIddictApplicationManager manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        #region BlazorWasm

        string clientId = configuration["Clients:BlazorWebAssembly:Id"]
            ?? throw new NullReferenceException($"[{nameof(Worker)}] Null reference exception. Variable: '{nameof(clientId)}' Value: '{null}'");
        string clientSecret = configuration["Clients:BlazorWebAssembly:Secret"]
            ?? throw new NullReferenceException($"[{nameof(Worker)}] Null reference exception. Variable: '{nameof(clientSecret)}' Value: '{null}'");
        string clientDisplayName = configuration["Clients:BlazorWebAssembly:DisplayName"]
            ?? throw new NullReferenceException($"[{nameof(Worker)}] Null reference exception. Variable: '{nameof(clientDisplayName)}' Value: '{null}'");

        if (await manager.FindByClientIdAsync(clientId) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                ConsentType = ConsentTypes.Explicit,
                DisplayName = clientDisplayName,
                RedirectUris =
                {
                    new Uri("https://localhost:5001/signin-oidc"),
                    new Uri("https://localhost:44132/signin-oidc")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:5001/signout-callback-oidc"),
                    new Uri("https://localhost:44132/signout-callback-oidc")
                },
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
                    Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess
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