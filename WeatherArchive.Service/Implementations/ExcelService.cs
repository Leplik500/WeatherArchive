using System.ComponentModel;
using System.Globalization;
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
                        throw new ArgumentException($"In sheet {sheet.SheetName} in row {row.RowNum} not all required fields are filled\n" +
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

    private CreateWeatherViewModel ParseRow(IRow row)
    {
        try
        {
            var date = row.GetCell(0).StringCellValue;
            var time = row.GetCell(1).StringCellValue;
            var dateTime = ParseDateTime(date, time);

            var temperature = GetDoubleFromCell(row.GetCell(2), "Temperature", -90, 60);
            var humidity = GetDoubleFromCell(row.GetCell(3), "Humidity", 0, 100);
            var dewPoint = GetDoubleFromCell(row.GetCell(4), "Dew Point", -60, 40);
            var pressure = GetIntFromCell(row.GetCell(5), "Pressure", 630, 820);

            var windDirectionCell = row.GetCell(6);
            var windDirection = ParseWindDirection(windDirectionCell.StringCellValue);

            var cell = row.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            var windSpeed = GetNullableIntFromCell(cell, "Wind Speed", 0, 410);
            cell = row.GetCell(8, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            var cloudCover = GetNullableIntFromCell(cell, "Cloud Cover", 0, 100);
            cell = row.GetCell(9, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            var cloudHeight = GetNullableIntFromCell(cell, "Cloud Height", 0, 3000);
            cell = row.GetCell(10, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            var visibility = GetNullableIntFromCell(cell, "Visibility", 0, 200);

            var weatherPhenomenon = row.GetCell(11, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue;
            if (!WeatherPhenomenonRegex().IsMatch(weatherPhenomenon) && !string.IsNullOrWhiteSpace(weatherPhenomenon))
                throw new ArgumentException("Weather phenomenon is not valid");

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
            logger.LogError("[ExcelService.ParseRow] row {RowNum} on sheet {SheetName}: {Message}", row.RowNum, row.Sheet.SheetName, e.Message);
            throw new ArgumentException($"[ExcelService.ParseRow] row {row.RowNum} on sheet {row.Sheet.SheetName}: {e.Message}");
        }
    }

    private static double GetDoubleFromCell(ICell cell, string name, int min, int max)
    {
        if (cell.CellType != CellType.Numeric)
            throw new ArgumentException($"{name} is not numeric");

        var value = cell.NumericCellValue;
        if (value > max || value < min)
            throw new ArgumentException($"{name} is out of range [{min}; {max}]");

        return value;
    }

    private static int GetIntFromCell(ICell cell, string name, int min, int max)
    {
        if (cell.CellType != CellType.Numeric)
            throw new ArgumentException($"{name} is not numeric");

        var value = cell.NumericCellValue;
        if (value > max || value < min)
            throw new ArgumentException($"{name} is out of range [{min}; {max}]");

        return (int) value;
    }

    private static int? GetNullableIntFromCell(ICell cell, string name, int min, int max)
    {
        int? value;
        if (cell.CellType != CellType.Numeric)
            value = null;
        else
            value = (int) cell.NumericCellValue;

        if (value > max || value < min)
            throw new ArgumentException($"{name} is out of range [{min}; {max}]");

        return value;
    }

    private static DateTime ParseDateTime(string date, string time)
    {
        var dateTimeConverter = new DateTimeConverter();
        var dateTimeString = date + " " + time;
        var culture = new CultureInfo("ru-RU");
        var moscowTime = (DateTime) (dateTimeConverter.ConvertFrom(null, culture, dateTimeString) ??
                                     throw new ArgumentException("Date and time are not valid"));

        var moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(moscowTime, moscowZone);

        if (utcTime > DateTime.Now)
            throw new ArgumentException("Time is in the future");

        if (utcTime < new DateTime(1900, 1, 1))
            throw new ArgumentException("Time is too old");

        return utcTime;
    }

    private static WindDirection? ParseWindDirection(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
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

    [GeneratedRegex(@"\b(?:град|позёмок|ливень|дождь|снег|дымка|морось|гроза|туман|небо|неба|радуга|северное сияние|облако|облака|облаков)\b",
            RegexOptions.IgnoreCase, "ru-RU")]
    private static partial Regex WeatherPhenomenonRegex();
}