using WeatherAlertTelegram.Domain;
using WeatherAlertTelegram.Domain.Weather;
using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Services;

public class ForecastService : IForecastService
{
    public AlertMessage? CreateAlert(WeatherObject weather, string city)
    {
        bool isNeedAlert = false;

        //TODO: Check weather (Лиза)

        if (isNeedAlert)
        {
            return new AlertMessage
            {
                City = city,
                Message = default
            };
        }
        else
        {
            return null;
        }
    }
}