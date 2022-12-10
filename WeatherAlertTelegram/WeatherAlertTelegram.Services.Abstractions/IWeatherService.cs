using WeatherAlertTelegram.Domain.Weather;

namespace WeatherAlertTelegram.Services.Abstractions;

public interface IWeatherService
{
    /// <summary>
    /// Gets current weather for specified city. 
    /// </summary>
    /// <param name="city"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WeatherObject> GetWeatherForCityAsync(string city, CancellationToken cancellationToken);
}