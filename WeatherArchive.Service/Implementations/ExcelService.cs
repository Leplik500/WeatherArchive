using Microsoft.Extensions.Logging;
using WeatherArchive.Domain.Entity;
using WeatherArchive.Domain.Response;
using WeatherArchive.Service.Interfaces;

namespace WeatherArchive.Service.Implementations;

public class ExcelService(ILogger<ExcelService> logger) : IExcelService
{
    public async Task<IBaseResponse<WeatherEntity>> ParseExcel(FileInfo fileInfo)
    {
        throw new NotImplementedException();
    }
}