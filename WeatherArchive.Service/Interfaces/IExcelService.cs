using WeatherArchive.Domain.Response;
using WeatherArchive.Domain.ViewModels.Weather;

namespace WeatherArchive.Service.Interfaces;

public interface IExcelService
{
    BaseResponse<IEnumerable<CreateWeatherViewModel>> ParseExcel(string filePath);
}