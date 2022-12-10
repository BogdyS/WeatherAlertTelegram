namespace WeatherAlertTelegram.Domain.Weather;

public class WeatherObject
{
    public WeatherInfo[] Weather { get; set; }
    public TemperatureInfo Main { get; set; }
    public string Name { get; set; }
}