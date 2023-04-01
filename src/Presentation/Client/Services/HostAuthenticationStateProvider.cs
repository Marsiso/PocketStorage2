using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Client.Services;

// original source https://github.com/berhir/BlazorWebAssemblyCookieAuth
public sealed class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    #region Private Fields

    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private readonly HttpClient _client;
    private readonly ILogger<HostAuthenticationStateProvider> _logger;
    private readonly NavigationManager _navigation;
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());
    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);

    #endregion Private Fields

    #region Private Methods

    private async Task<ClaimsPrincipal> FetchUser()
    {
        ApplicationUserInfo? user = default;

        try
        {
            _logger.LogInformation("{clientBaseAddress}", _client.BaseAddress?.ToString());
            user = await _client.GetFromJsonAsync<ApplicationUserInfo>("api/user");
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, $"[{nameof(HostAuthenticationStateProvider)}] Fetch user failure.");
        }

        if (user == null || !user.IsAuthenticated)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        ClaimsIdentity identity = new ClaimsIdentity(
            nameof(HostAuthenticationStateProvider),
            user.NameClaimType,
            user.RoleClaimType);

        if (user.Claims != null)
        {
            identity.AddClaims(user.Claims.Select(claimValue => new Claim(claimValue.Type, claimValue.Value)));
        }

        return new ClaimsPrincipal(identity);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        if (useCache && dateTimeOffset < _userLastCheck + _userCacheRefreshInterval)
        {
            _logger.LogDebug($"[{nameof(HostAuthenticationStateProvider)}] Retrieving user from cache.");
            return _cachedUser;
        }

        _logger.LogDebug($"[{nameof(HostAuthenticationStateProvider)}] Fetching user.");
        _cachedUser = await FetchUser();
        _userLastCheck = dateTimeOffset;

        return _cachedUser;
    }

    #endregion Private Methods

    #region Public Constructors

    public HostAuthenticationStateProvider(NavigationManager navigation, HttpClient client, ILogger<HostAuthenticationStateProvider> logger)
    {
        _navigation = navigation;
        _client = client;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        => new AuthenticationState(await GetUser(useCache: true));

    public void SignIn(string? customReturnUrl = null)
    {
        var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
        var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
        Uri logInUrl = _navigation.ToAbsoluteUri($"{AuthorizationDefaults.LogInPath}?returnUrl={encodedReturnUrl}");
        _navigation.NavigateTo(logInUrl.ToString(), true);
    }

    #endregion Public Methods
}