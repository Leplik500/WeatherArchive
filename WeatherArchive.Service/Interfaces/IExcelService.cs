using WeatherArchive.Domain.Entity;
using WeatherArchive.Domain.Response;

namespace WeatherArchive.Service.Interfaces;

public interface IExcelService
{
    Task<IBaseResponse<WeatherEntity>> ParseExcel(FileInfo fileInfo);
}