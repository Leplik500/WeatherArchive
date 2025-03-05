using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherArchive.Models;

namespace WeatherArchive.Controllers;

public class WeatherController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Upload()
    {
        return View();
    }

    public IActionResult Display()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}