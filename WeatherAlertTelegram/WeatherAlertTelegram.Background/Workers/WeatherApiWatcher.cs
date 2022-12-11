using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Background.Workers;

public class WeatherApiWatcher
{
    public const string TaskName = "weather-api-watcher";

    private readonly IWeatherService _weatherService;
    private readonly IAccountService _accountService;
    private readonly IForecastService _forecastService;

    public WeatherApiWatcher(IWeatherService weatherService, IAccountService accountService, IForecastService forecastService)
    {
        _weatherService = weatherService;
        _accountService = accountService;
        _forecastService = forecastService;
    }

    public async Task ExecuteAsync()
    {
        var users = await _accountService.GetUsersAsync(CancellationToken.None);

        var cities = users.Select(x => x.City).Distinct().ToList();

        foreach (var city in cities)
        {
            try
            {
                var weather = await _weatherService.GetWeatherForCityAsync(city, CancellationToken.None);
                var message = _forecastService.CreateAlert(weather, city);
                if (message != null)
                {
                    //TODO: Send messages to users
                }
            }
            catch
            {
                continue;
            }
        }
    }
}