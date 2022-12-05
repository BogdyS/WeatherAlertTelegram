namespace WeatherAlertTelegram.Controllers.TestControllers.Dto;

public class UserListEntryDto
{
    public int Id { get; set; }

    public int ChatId { get; set; }
    public string UserName { get; set; }

    public string City { get; set; }
}