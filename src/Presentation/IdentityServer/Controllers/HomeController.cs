using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityServer.Controllers;

public sealed class HomeController : Controller
{
    #region Private Fields

    private readonly ILogger<HomeController> _logger;

    #endregion Private Fields

    #region Public Constructors

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    #endregion Public Methods
}