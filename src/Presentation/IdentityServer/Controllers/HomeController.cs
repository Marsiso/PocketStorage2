using IdentityServer.Controllers.Web.Base;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityServer.Controllers;

public sealed class HomeController : BaseWebController<HomeController>
{
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