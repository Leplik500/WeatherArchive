using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeatherArchive.DAL.Interfaces;
using WeatherArchive.Domain.Entity;
using WeatherArchive.Domain.Enum;
using WeatherArchive.Domain.Response;
using WeatherArchive.Domain.ViewModels.Weather;
using WeatherArchive.Service.Interfaces;

namespace WeatherArchive.Service.Implementations;

public class WeatherService(IBaseRepository<WeatherEntity> repository, ILogger<WeatherService> logger)
        : IWeatherService
{
    public async Task<IBaseResponse<WeatherEntity>> Create(CreateWeatherViewModel model)
    {
        try
        {
            logger.LogInformation("Creating Weather with and {WeatherPhenomenon} and {Date}",
                    model.WeatherPhenomenon, model.Date);

            var entity = new WeatherEntity
            {
                Date = model.Date,
                WeatherPhenomenon = model.WeatherPhenomenon,
                WindDirection = model.WindDirection,
                WindSpeed = model.WindSpeed,
                Pressure = model.Pressure,
                Humidity = model.Humidity,
                Temperature = model.Temperature,
                Visibility = model.Visibility,
                CloudCover = model.CloudCover,
                CloudHeight = model.CloudHeight,
                DewPoint = model.DewPoint
            };

            await repository.Create(entity);
            return new BaseResponse<WeatherEntity>
            {
                Description = $"Запись погоды с {model.WeatherPhenomenon} {model.Date} была создана",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            logger.LogError("[WeatherService.Create] {Message}", e.Message);
            return new BaseResponse<WeatherEntity>
            {
                Description = e.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<WeatherViewModel>>> GetAll()
    {
        try
        {
            var weathers = await repository
                    .GetAll()
                    .Select(w => new WeatherViewModel
                    {
                        Date = w.Date,
                        WeatherPhenomenon = w.WeatherPhenomenon,
                        WindDirection = w.WindDirection,
                        WindSpeed = w.WindSpeed,
                        Pressure = w.Pressure,
                        Humidity = w.Humidity,
                        Temperature = w.Temperature,
                        Visibility = w.Visibility,
                        CloudCover = w.CloudCover,
                        CloudHeight = w.CloudHeight,
                        DewPoint = w.DewPoint
                    })
                    .ToListAsync();

            return new BaseResponse<IEnumerable<WeatherViewModel>>
            {
                Data = weathers,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            logger.LogError("[WeatherService.GetAll] {Message}", e.Message);
            return new BaseResponse<IEnumerable<WeatherViewModel>>
            {
                Description = e.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<WeatherEntity>>> CreateMultiple(IEnumerable<CreateWeatherViewModel> models)
    {
        var weatherEntities = new List<WeatherEntity>();
        foreach (var weather in models)
        {
            var response = await Create(weather);
            if (response.StatusCode != StatusCode.OK)
                return new BaseResponse<IEnumerable<WeatherEntity>>
                {
                    Description = response.Description,
                    StatusCode = StatusCode.InternalServerError
                };

            weatherEntities.Add(response.Data);
        }

        return new BaseResponse<IEnumerable<WeatherEntity>>
        {
            Data = weatherEntities,
            StatusCode = StatusCode.OK
        };
    }
}