namespace Client.Services.Abstractions;

public interface IAntiforgeryHttpClientFactory
{
    #region Public Methods

    Task<HttpClient> CreateClientAsync(string clientName = AuthorizationDefaults.AuthorizedClientName);

    #endregion Public Methods
}