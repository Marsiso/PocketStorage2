using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Web.Base;

public class BaseWebController<T> : Controller
{
    #region Private Fields

    private ILogger<T>? _logger;

    #endregion Private Fields

    #region Protected Properties

    protected ILogger<T>? Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>?>();

    #endregion Protected Properties
}