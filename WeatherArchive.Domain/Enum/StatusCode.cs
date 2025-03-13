namespace WeatherArchive.Domain.Enum;

public enum StatusCode
{
    OK = 200,
    InternalServerError = 500,
    FileNotSupported = 415,
    InvalidData = 422
}