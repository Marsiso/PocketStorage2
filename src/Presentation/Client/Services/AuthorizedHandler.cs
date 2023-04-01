using System.Net;

namespace Client.Services;

// original source https://github.com/berhir/BlazorWebAssemblyCookieAuth
public sealed class AuthorizedHandler : DelegatingHandler
{
    #region Private Fields

    private readonly HostAuthenticationStateProvider _authenticationStateProvider;

    #endregion Private Fields

    #region Protected Methods

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;
        if (authState.User.Identity != null && !authState.User.Identity.IsAuthenticated)
        {
            // If user is not authenticated, immediately set response status to 401 Unauthorized.
            responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        else
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            // If server returned 401 Unauthorized, redirect to login page.
            _authenticationStateProvider.SignIn();
        }

        return responseMessage;
    }

    #endregion Protected Methods

    #region Public Constructors

    public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider)
            => _authenticationStateProvider = authenticationStateProvider;

    #endregion Public Constructors
}