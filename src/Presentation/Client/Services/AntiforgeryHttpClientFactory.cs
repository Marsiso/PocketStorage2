namespace Client.Services;

public sealed class AntiforgeryHttpClientFactory : IAntiforgeryHttpClientFactory
{
    #region Private Fields

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJSRuntime _jSRuntime;

    #endregion Private Fields

    #region Public Constructors

    public AntiforgeryHttpClientFactory(IHttpClientFactory httpClientFactory, IJSRuntime jSRuntime)
    {
        _httpClientFactory = httpClientFactory;
        _jSRuntime = jSRuntime;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task<HttpClient> CreateClientAsync(string clientName = AuthorizationDefaults.AuthorizedClientName)
    {
        var token = await _jSRuntime.InvokeAsync<string>("getAntiForgeryToken");

        var client = _httpClientFactory.CreateClient(clientName);
        client.DefaultRequestHeaders.Add(AntiforgeryDefaults.HeaderName, token);

        return client;
    }

    #endregion Public Methods
}