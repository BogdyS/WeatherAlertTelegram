﻿namespace WeatherAlertTelegram.Domain;

public class UserListItem
{
    public int Id { get; set; }

    public long ChatId { get; set; }
    public string UserName { get; set; }

    public string City { get; set; }
}