using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherAlertTelegram.Domain.Exceptions;
using WeatherAlertTelegram.Domain.Weather;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Options;

namespace WeatherAlertTelegram.Services;

public class WeatherService : IWeatherService
{
    private const string Routing = "http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&appid={1}&lang=ru";
    private readonly WeatherApiOptions _options;
    private readonly HttpClient _httpClient;

    public WeatherService(IOptions<WeatherApiOptions> options, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<WeatherObject?> GetWeatherForCityAsync(string city, CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(Routing, city ,_options.ApiKey));

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AppException("Something went wrong. Try again later.");
        }

        var body = await new StreamReader(await response.Content.ReadAsStreamAsync(cancellationToken)).ReadToEndAsync();

        var @object = JsonConvert.DeserializeObject<WeatherObject>(body);

        return @object;
    }
}