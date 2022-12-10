using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Background.Workers;

public class WeatherApiWatcher
{
    public const string TaskName = "weather-api-watcher";

    private readonly IWeatherService _weatherService;
    private readonly IAccountService _accountService;

    public WeatherApiWatcher(IWeatherService weatherService, IAccountService accountService)
    {
        _weatherService = weatherService;
        _accountService = accountService;
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
                //TODO: Implement weather's checks (Лиза)
            }
            catch
            {
                continue;
            }
            //TODO: Send messages to users
        }
    }
}