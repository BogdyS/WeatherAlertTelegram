using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Background.Workers;

public class WeatherApiWatcher
{
    public const string TaskName = "weather-api-watcher";

    private readonly IWeatherService _weatherService;
    private readonly IAccountService _accountService;
    private readonly IForecastService _forecastService;
    private readonly ITelegramService _telegramService;

    public WeatherApiWatcher(
        IWeatherService weatherService,
        IAccountService accountService,
        IForecastService forecastService,
        ITelegramService telegramService)
    {
        _weatherService = weatherService;
        _accountService = accountService;
        _forecastService = forecastService;
        _telegramService = telegramService;
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
                    var usersToSend = users
                        .Where(x => x.City == city)
                        .Select(x => x.ChatId)
                        .ToList();

                    await _telegramService.SendMessageAsync(usersToSend, message.Message, CancellationToken.None);
                }
            }
            catch
            {
                continue;
            }
        }
    }
}