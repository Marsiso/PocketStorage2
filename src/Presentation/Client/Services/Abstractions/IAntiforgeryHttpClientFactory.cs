namespace Client.Services.Abstractions;

public interface IAntiforgeryHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string clientName = AuthorizationDefaults.AuthorizedClientName);
}
