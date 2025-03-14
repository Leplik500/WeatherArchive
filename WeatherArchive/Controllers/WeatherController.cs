using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherArchive.Domain.Enum;
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
                Description = "Файл не поддерживается",
                StatusCode = Domain.Enum.StatusCode.FileNotSupported
            };

        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetRandomFileName());
        await using var filestream = new FileStream(filepath, FileMode.Create);
        await file.CopyToAsync(filestream);
        var response = excelService.ParseExcel(filepath);
        if (response.StatusCode is Domain.Enum.StatusCode.OK or Domain.Enum.StatusCode.PartialSuccess)
        {
            if (response.Data != null)
                await weatherService.CreateMultiple(response.Data);
            else
                return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
                {
                    Description = "Произошла ошибка при обработке данных",
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };

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
        List<StatusCode> statusCodes = [];
        foreach (var file in files)
        {
            var result = await UploadHandle(file);
            statusCodes.Add(result.StatusCode);
            if (result.Description != null)
                resultDescriptions.Add($"{file.FileName}:\n{result.Description}\n");
        }

        if (statusCodes.TrueForAll(code => code.Equals(Domain.Enum.StatusCode.OK)))
            return Ok(new {description = "Все файлы успешно загружены и разобраны"});

        var hasPartial = statusCodes.Any(c => c is Domain.Enum.StatusCode.OK or Domain.Enum.StatusCode.PartialSuccess);
        var status = hasPartial ? "PartialSuccess" : "CompleteFailure";

        return BadRequest(new {description = resultDescriptions, status});
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