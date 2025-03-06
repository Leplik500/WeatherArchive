using WeatherArchive.DAL.Interfaces;
using WeatherArchive.Domain.Entity;

namespace WeatherArchive.DAL.Repositories;

public class WeatherRepository : IBaseRepository<WeatherEntity>
{
    public async Task Create(WeatherEntity entity)
    {
        throw new NotImplementedException();
    }

    public IQueryable<WeatherEntity> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task Delete(WeatherEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task Update(WeatherEntity entity)
    {
        throw new NotImplementedException();
    }
}