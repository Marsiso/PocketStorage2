﻿using Microsoft.AspNetCore.Components;
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
        ApplicationUserInfo? user = null;

        try
        {
            _logger.LogInformation("{clientBaseAddress}", _client.BaseAddress?.ToString());
            user = await _client.GetFromJsonAsync<ApplicationUserInfo>("api/user");
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Fetching user failed.");
        }

        if (user == null || !user.IsAuthenticated)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var identity = new ClaimsIdentity(
            nameof(HostAuthenticationStateProvider),
            user.NameClaimType,
            user.RoleClaimType);

        if (user.Claims != null)
        {
            identity.AddClaims(user.Claims.Select(c => new Claim(c.Type, c.Value)));
        }

        return new ClaimsPrincipal(identity);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
        {
            _logger.LogDebug("Taking user from cache");
            return _cachedUser;
        }

        _logger.LogDebug("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

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
        var logInUrl = _navigation.ToAbsoluteUri($"{AuthorizationDefaults.LogInPath}?returnUrl={encodedReturnUrl}");
        _navigation.NavigateTo(logInUrl.ToString(), true);
    }

    #endregion Public Methods
}