using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WeatherArchive.Domain.Enum;
using WeatherArchive.Domain.Extensions;
using WeatherArchive.Domain.Response;
using WeatherArchive.Domain.ViewModels.Weather;
using WeatherArchive.Service.Interfaces;

namespace WeatherArchive.Service.Implementations;

public partial class ExcelService(ILogger<ExcelService> logger) : IExcelService
{
    public BaseResponse<IEnumerable<CreateWeatherViewModel>> ParseExcel(string filePath)
    {
        try
        {
            var book = OpenExcel(filePath);
            if (book == null)
                return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
                {
                    Description = "File not supported",
                    StatusCode = StatusCode.FileNotSupported
                };

            var weathers = new List<CreateWeatherViewModel>();
            for (var i = 0; i < book.NumberOfSheets; i++)
            {
                var sheet = book.GetSheetAt(i);
                for (var j = 4; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);
                    if (row == null)
                        continue;

                    var entity = ParseRow(row);
                    if (entity == null)
                        throw new ArgumentException($"In row {row.RowNum} not all required fields are filled\n" +
                                                    $"Required fieds are Date, Time, Temperature, Humidity, Dew Point, Pressure");
                    

                    weathers.Add(entity);
                }
            }

            return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
            {
                Data = weathers,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            logger.LogError("[ExcelService.ParseExcel] {Message}", e.Message);
            return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
            {
                Description = e.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    private CreateWeatherViewModel? ParseRow(IRow row)
    {
        try
        {
            //TODO: поэкспериментировать с GetCell и MissingCellPolicy, что оно будет возвращать в зависимости
            // от разных значений MissingCellPolicy?
            var date = row.GetCell(0).DateOnlyCellValue;
            var time = row.GetCell(1).TimeOnlyCellValue;
            if (date is null)
                throw new ArgumentException($"Date on row {row.RowNum} is null");

            if (time is null)
                throw new ArgumentException($"Time on row {row.RowNum} is null");

            var dateTime = new DateTime((DateOnly) date, (TimeOnly) time, DateTimeKind.Local);
            if (dateTime > DateTime.Now)
                throw new ArgumentException($"Time on row {row.RowNum} is in the future");

            if (dateTime < new DateTime(1900, 1, 1))
                throw new ArgumentException($"Time on row {row.RowNum} is too old");

            var windDirectionCell = row.GetCell(6);
            var windDirection = ParseWindDirection(windDirectionCell.StringCellValue);

            var temperature = row.GetCell(2).NumericCellValue;
            if (temperature is > 60 or < -90)
                throw new ArgumentException($"Temperature on row {row.RowNum} is out of range [-90; 60]");

            var humidity = row.GetCell(3).NumericCellValue;
            if (humidity is > 100 or < 0)
                throw new ArgumentException($"Humidity on row {row.RowNum} is out of range [0; 100]");

            var dewPoint = row.GetCell(4).NumericCellValue;
            if (dewPoint is > 40 or < -60)
                throw new ArgumentException($"Dew point on row {row.RowNum} is out of range [-60; 40]");

            var pressure = (int) row.GetCell(5).NumericCellValue;
            if (pressure is > 820 or < 630)
                throw new ArgumentException($"Pressure on row {row.RowNum} is out of range [820; 630]");

            var windSpeed = (int) row.GetCell(7).NumericCellValue;
            if (windSpeed is > 410 or < 0)
                throw new ArgumentException($"Wind speed on row {row.RowNum} is out of range [0; 410]");

            var cloudCover = (int) row.GetCell(8).NumericCellValue;
            if (cloudCover is > 100 or < 0)
                throw new ArgumentException($"Cloud cover on row {row.RowNum} is out of range [0; 100]");

            var cloudHeight = (int) row.GetCell(9).NumericCellValue;
            if (cloudHeight is > 3000 or < 0)
                throw new ArgumentException($"Cloud height on row {row.RowNum} is out of range [0; 3000]");

            var visibility = (int) row.GetCell(10).NumericCellValue;
            if (visibility is > 200 or < 0)
                throw new ArgumentException($"Visibility on row {row.RowNum} is out of range [0; 200]");

            var weatherPhenomenon = row.GetCell(11).StringCellValue;
            if (!WeatherPhenomenonRegex().IsMatch(weatherPhenomenon))
                throw new ArgumentException($"Weather phenomenon on row {row.RowNum} is not valid");

            return new CreateWeatherViewModel
            {
                Date = dateTime,
                Temperature = temperature,
                Humidity = humidity,
                DewPoint = dewPoint,
                Pressure = pressure,
                WindDirection = windDirection,
                WindSpeed = windSpeed,
                CloudCover = cloudCover,
                CloudHeight = cloudHeight,
                Visibility = visibility,
                WeatherPhenomenon = weatherPhenomenon
            };
        }
        catch (Exception e)
        {
            logger.LogError("[ExcelService.ParseRow] {Message}", e.Message);
            return null;
        }
    }

    private static WindDirection? ParseWindDirection(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        var normalized = NormalizeInput(input);

        foreach (WindDirection direction in Enum.GetValues(typeof(WindDirection)))
        {
            var displayName = direction.GetDisplayName();
            if (displayName.Equals(normalized, StringComparison.OrdinalIgnoreCase))
                return direction;
        }

        throw new ArgumentException($"Unknown wind direction: {input}");
    }

    private static string NormalizeInput(string input)
    {
        return string.Join(", ", input.Split([','], StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrEmpty(part)));
    }


    private IWorkbook? OpenExcel(string filePath)
    {
        IWorkbook? book = null;
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                // xslx
                book = new XSSFWorkbook(fileStream, true);
            }
            catch
            {
                book = null;
            }

            // xls
            book ??= new HSSFWorkbook(fileStream);
            return book;
        }
        catch (Exception ex)
        {
            logger.LogError("[ExcelService.OpenExcel] {Message}", ex.Message);
            return book;
        }
    }

    [GeneratedRegex(@"\b(?:град|позёмок|ливень|дождь|снег|дымка|морось|гроза|туман)\b", RegexOptions.IgnoreCase, "ru-RU")]
    private static partial Regex WeatherPhenomenonRegex();
}