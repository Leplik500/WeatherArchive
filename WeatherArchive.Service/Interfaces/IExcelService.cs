using WeatherArchive.Domain.Response;
using WeatherArchive.Domain.ViewModels.Weather;

namespace WeatherArchive.Service.Interfaces;

public interface IExcelService
{
    Task<IBaseResponse<CreateWeatherViewModel>> Upload(IEnumerable<FileInfo> files);
}