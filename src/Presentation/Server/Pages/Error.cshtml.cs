namespace Server.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    #region Private Fields

    private readonly ILogger<ErrorModel> _logger;

    #endregion Private Fields

    #region Public Constructors

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Properties

    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    #endregion Public Properties

    #region Public Methods

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }

    #endregion Public Methods
}