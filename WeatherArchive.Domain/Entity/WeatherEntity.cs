using System.ComponentModel.DataAnnotations;
using WeatherArchive.Domain.Enum;

namespace WeatherArchive.Domain.Entity;

public class WeatherEntity
{
    [Key] public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double DewPoint { get; set; }
    public int Pressure { get; set; }
    public WindDirection? WindDirection { get; set; }
    public int? WindSpeed { get; set; }
    public int? CloudCover { get; set; }
    public int? CloudHeight { get; set; }
    public int? Visibility { get; set; }
    public string? WeatherPhenomenon { get; set; }
}