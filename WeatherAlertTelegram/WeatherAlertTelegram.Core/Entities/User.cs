namespace WeatherAlertTelegram.Core.Entities;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string City { get; set; }

    public long ChatId { get; set; }
}