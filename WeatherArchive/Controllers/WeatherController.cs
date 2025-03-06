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

    [HttpPost]
    public async Task<IActionResult> UploadHandle(List<IFormFile> files)
    {
        foreach (var file in files)
        {
            if (file.Length > 0 &&
                (file.FileName.EndsWith("xsl") ||
                 file.FileName.EndsWith("xlsx")))
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetRandomFileName());
                await using var filestream = new FileStream(filepath, FileMode.Create);
                await file.CopyToAsync(filestream);
            }
            else
            {
                return BadRequest("File not supported");
            }
        }

        // TODO:redirect to parse method and then remove files
        return RedirectToAction("Upload");
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