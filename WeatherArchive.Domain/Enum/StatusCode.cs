namespace WeatherArchive.Domain.Enum;

public enum StatusCode
{
    None = 0,
    OK = 200,
    InternalServerError = 500,
    FileNotSupported = 415,
    InvalidData = 422,
    PartialSuccess = 207
}