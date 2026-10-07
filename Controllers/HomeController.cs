using DiagnosticsActivity = System.Diagnostics.Activity;
using Microsoft.AspNetCore.Mvc;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;

namespace AcxiomCRM.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = DiagnosticsActivity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
