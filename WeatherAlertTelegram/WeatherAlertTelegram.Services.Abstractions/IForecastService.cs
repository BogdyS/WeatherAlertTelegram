using WeatherAlertTelegram.Domain;
using WeatherAlertTelegram.Domain.Weather;

namespace WeatherAlertTelegram.Services.Abstractions;

public interface IForecastService
{
    AlertMessage CreateAlert(WeatherObject weather, string city);
}