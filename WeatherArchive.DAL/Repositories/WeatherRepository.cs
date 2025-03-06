using WeatherArchive.DAL.Interfaces;
using WeatherArchive.Domain.Entity;

namespace WeatherArchive.DAL.Repositories;

public class WeatherRepository(AppDbContext context) : IBaseRepository<WeatherEntity>
{
    public async Task Create(WeatherEntity entity)
    {
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<WeatherEntity> GetAll()
    {
        return context.WeatherEntities;
    }

    public async Task Delete(WeatherEntity entity)
    {
        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<WeatherEntity> Update(WeatherEntity entity)
    {
        context.WeatherEntities.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}