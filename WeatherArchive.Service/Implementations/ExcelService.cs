using System.ComponentModel;
using System.Globalization;
using System.Text;
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
using Exception = System.Exception;

namespace WeatherArchive.Service.Implementations;

public partial class ExcelService(ILogger<ExcelService> logger) : IExcelService
{
    public BaseResponse<IEnumerable<CreateWeatherViewModel>> ParseExcel(string filePath)
    {
        var errors = new StringBuilder();
        List<CreateWeatherViewModel> weathers = [];

        try
        {
            using var book = OpenExcel(filePath);
            if (book == null)
                return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
                {
                    Description = "Файл не поддерживается",
                    StatusCode = StatusCode.FileNotSupported
                };

            for (var i = 0; i < book.NumberOfSheets; i++)
            {
                var sheet = book.GetSheetAt(i);
                for (var j = 4; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);
                    if (row == null)
                        continue;

                    try
                    {
                        var entity = ParseRow(row);
                        weathers.Add(entity);
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine($"Лист {sheet.SheetName}, строка {row.RowNum}: {ex.Message}");
                    }
                }
            }

            return errors.Length switch
            {
                > 0 when weathers.Count > 0 => new BaseResponse<IEnumerable<CreateWeatherViewModel>>
                {
                    Data = weathers, Description = errors.ToString(), StatusCode = StatusCode.PartialSuccess
                },
                > 0 when weathers.Count == 0 => new BaseResponse<IEnumerable<CreateWeatherViewModel>>
                {
                    Description = errors.ToString(), StatusCode = StatusCode.InvalidData
                },
                _ => new BaseResponse<IEnumerable<CreateWeatherViewModel>> {Data = weathers, StatusCode = StatusCode.OK}
            };
        }
        catch (Exception ex)
        {
            logger.LogError("[ExcelService.ParseExcel] {Message}", ex.Message);
            return new BaseResponse<IEnumerable<CreateWeatherViewModel>>
            {
                Description = $"Критическая ошибка: {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    private static CreateWeatherViewModel ParseRow(IRow row)
    {
        var date = row.GetCell(0).StringCellValue;
        var time = row.GetCell(1).StringCellValue;
        var dateTime = ParseDateTime(date, time);

        var temperature = GetDoubleFromCell(row.GetCell(2), "Температура", -90, 60);
        var humidity = GetDoubleFromCell(row.GetCell(3), "Влажность", 0, 101);
        var dewPoint = GetDoubleFromCell(row.GetCell(4), "Точка росы", -60, 40);
        var pressure = GetIntFromCell(row.GetCell(5), "Давление", 630, 820);

        var windDirectionCell = row.GetCell(6);
        var windDirection = ParseWindDirection(windDirectionCell.StringCellValue);

        var cell = row.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK);
        var windSpeed = GetNullableIntFromCell(cell, "Скорость ветра", 0, 410);
        cell = row.GetCell(8, MissingCellPolicy.CREATE_NULL_AS_BLANK);
        var cloudCover = GetNullableIntFromCell(cell, "Облачность", 0, 100);
        cell = row.GetCell(9, MissingCellPolicy.CREATE_NULL_AS_BLANK);
        var cloudHeight = GetNullableIntFromCell(cell, "Нижняя граница облачности", 0, 3000);
        cell = row.GetCell(10, MissingCellPolicy.CREATE_NULL_AS_BLANK);
        var visibility = GetNullableIntFromCell(cell, "Видимость", 0, 200);

        var weatherPhenomenon = row.GetCell(11, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue;
        if (!WeatherPhenomenonRegex().IsMatch(weatherPhenomenon) && !string.IsNullOrWhiteSpace(weatherPhenomenon))
            throw new ArgumentException("Погодное явление указано неверно");

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

    private static double GetDoubleFromCell(ICell cell, string name, int min, int max)
    {
        if (cell.CellType != CellType.Numeric)
            throw new ArgumentException($"{name} не является числовым значением");

        var value = cell.NumericCellValue;
        if (value > max || value < min)
            throw new ArgumentException($"{name} вне допустимого диапазона [{min}; {max}], значение {value}");

        return value;
    }

    private static int GetIntFromCell(ICell cell, string name, int min, int max)
    {
        if (cell.CellType != CellType.Numeric)
            throw new ArgumentException($"{name} не является числовым значением");

        var value = cell.NumericCellValue;
        if (value > max || value < min)
            throw new ArgumentException($"{name} вне допустимого диапазона [{min}; {max}], значение {value}");

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
            throw new ArgumentException($"{name} вне допустимого диапазона [{min}; {max}], значение {value}");

        return value;
    }

    private static DateTime ParseDateTime(string date, string time)
    {
        var dateTimeConverter = new DateTimeConverter();
        var dateTimeString = date + " " + time;
        var culture = new CultureInfo("ru-RU");
        var moscowTime = (DateTime) (dateTimeConverter.ConvertFrom(null, culture, dateTimeString) ??
                                     throw new ArgumentException("Дата и время указаны неверно"));

        var moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(moscowTime, moscowZone);

        if (utcTime > DateTime.Now)
            throw new ArgumentException("Время задано в будущем");

        if (utcTime < new DateTime(1900, 1, 1))
            throw new ArgumentException("Время слишком давнее");

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

        throw new ArgumentException($"Неизвестное направление ветра: {input}");
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
                book = new XSSFWorkbook(fileStream, true);
            }
            catch
            {
                book = null;
            }

            book ??= new HSSFWorkbook(fileStream);
            return book;
        }
        catch (Exception ex)
        {
            logger.LogError("[ExcelService.OpenExcel] {Message}", ex.Message);
            return book;
        }
    }

    [GeneratedRegex(
            @"\b(?:град|позёмок|ливень|дождь|снег|дымка|морось|гроза|туман|небо|неба|радуга|северное сияние|облако|облака|облаков|иглы|крупа|иней|снегопад|изморозь|мороз|налёт|налет|гололед|гололёд|гололедица|пыль|буря|молния|гром|мгла|зарница|огни|радуга|гало|мираж|столб|заря|глория|шквал|вихрь|смерч|кристаллы|зёрна|зерна)\b",
            RegexOptions.IgnoreCase, "ru-RU")]
    private static partial Regex WeatherPhenomenonRegex();
}