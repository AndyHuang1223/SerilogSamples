using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebApplicationSample.Models;

namespace WebApplicationSample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public HomeController(ILogger<HomeController> logger, IDiagnosticContext diagnosticContext)
    {
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public IActionResult Index()
    {
        _diagnosticContext.Set("CatalogLoadTime", 1423);
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Privacy page visited");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}