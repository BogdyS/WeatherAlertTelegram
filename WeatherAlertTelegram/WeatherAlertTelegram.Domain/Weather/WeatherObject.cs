namespace WeatherAlertTelegram.Domain.Weather;

public class WeatherObject
{
    public WeatherInfo[] Weather { get; set; }
    public MainInfo Main { get; set; }
    public string Name { get; set; }

    public float Visibility { get; set; }

    public WindInfo Wind { get; set; }
}