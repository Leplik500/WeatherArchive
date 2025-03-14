using WeatherArchive.Domain.Entity;

namespace WeatherArchive.DAL.Interfaces;

public interface IBaseRepository<T>
{
    Task Create(T entity);
    IQueryable<T> GetAll();
    Task Delete(T entity);
    Task<WeatherEntity> Update(T entity);
}