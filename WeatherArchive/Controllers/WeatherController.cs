using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherArchive.Domain.Response;
using WeatherArchive.Domain.ViewModels.Weather;
using WeatherArchive.Models;
using WeatherArchive.Service.Interfaces;

namespace WeatherArchive.Controllers;

public class WeatherController(IExcelService excelService, IWeatherService weatherService) : Controller
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
    private async Task<BaseResponse<IEnumerable<CreateWeatherViewModel>>> UploadHandle(IFormFile file)
    {
        if (!(file.Length > 0 &
              (file.FileName.EndsWith("xsl") ||
               file.FileName.EndsWith("xlsx"))))
            return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
            {
                Description = "File not supported",
                StatusCode = Domain.Enum.StatusCode.FileNotSupported
            };

        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetRandomFileName());
        await using var filestream = new FileStream(filepath, FileMode.Create);
        await file.CopyToAsync(filestream);
        var response = excelService.ParseExcel(filepath);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            await weatherService.CreateMultiple(response.Data);
            System.IO.File.Delete(filepath);
            return response;
        }

        System.IO.File.Delete(filepath);
        return response;
    }

    [HttpPost]
    public async Task<IActionResult> MultipleUploadHandle(List<IFormFile> files)
    {
        List<string> resultDescriptions = [];
        var isSuccesful = true;
        foreach (var file in files)
        {
            var result = await UploadHandle(file);
            if (result.StatusCode != Domain.Enum.StatusCode.OK)
                isSuccesful = false;

            resultDescriptions.Add($"{file.FileName}: {result.Description};\n");
        }

        if (isSuccesful)
            return Ok(new {description = "All files loaded and parsed successfully"});

        return BadRequest(new {description = resultDescriptions});
    }


    public IActionResult Display()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetIssues()
    {
        var response = await weatherService.GetAll();
        return Json(new {data = response.Data});
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}