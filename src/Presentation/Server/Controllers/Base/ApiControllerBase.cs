using Domain.Data;

namespace Server.Controllers.Base;

[ApiController]
public abstract class ApiControllerBase<T> : ControllerBase
{
    #region Private Fields

    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<T> _logger;

    #endregion Private Fields

    #region Public Constructors

    public ApiControllerBase(ILogger<T> logger, ApplicationDbContext databaseContext)
    {
        _logger = logger;
        _databaseContext = databaseContext;
    }

    #endregion Public Constructors
}