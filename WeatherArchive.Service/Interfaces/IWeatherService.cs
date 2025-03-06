using WeatherArchive.Domain.Entity;
using WeatherArchive.Domain.Response;
using WeatherArchive.Domain.ViewModels.Weather;

namespace WeatherArchive.Service.Interfaces;

public interface IWeatherService
{
    Task<IBaseResponse<WeatherEntity>> Create(CreateWeatherViewModel model);
    Task<IBaseResponse<IEnumerable<WeatherViewModel>>> GetAll();
}