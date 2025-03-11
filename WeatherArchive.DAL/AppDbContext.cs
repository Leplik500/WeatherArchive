using Microsoft.EntityFrameworkCore;
using WeatherArchive.Domain.Entity;

namespace WeatherArchive.DAL;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<WeatherEntity> WeatherEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherEntity>();
        base.OnModelCreating(modelBuilder);
    }
}