using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Web.Base;

public class BaseWebController<T> : Controller
{
    private ILogger<T>? _logger;
    protected ILogger<T>? Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>?>();
}
