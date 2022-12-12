using WeatherAlertTelegram.Core.Enums;

namespace WeatherAlertTelegram.Core.Entities;

public class PendingRegistrations
{
    public int Id { get; set; }

    public long ChatId { get; set; }
    
    public string UserName { get; set; }

    public string City { get; set; }

    public RegistrationSteps Step { get; set; }
}