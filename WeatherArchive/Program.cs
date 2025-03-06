using Microsoft.EntityFrameworkCore;
using WeatherArchive.DAL;
using WeatherArchive.DAL.Interfaces;
using WeatherArchive.DAL.Repositories;
using WeatherArchive.Domain.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBaseRepository<WeatherEntity>, WeatherRepository>();

builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Postgres");
            options.UseNpgsql(connectionString);
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
        "default",
        "",
        new {controller = "Weather", action = "Index"});

app.MapControllerRoute(
        "action",
        "{action=Index}",
        new {controller = "Weather", action = "Index"});

app.Run();