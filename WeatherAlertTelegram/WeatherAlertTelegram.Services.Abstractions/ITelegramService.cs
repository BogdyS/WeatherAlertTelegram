using Telegram.Bot.Types;

namespace WeatherAlertTelegram.Services.Abstractions;

public interface ITelegramService
{
    Task HandleAsync(Update update, CancellationToken cancellationToken);
    Task SetWebHookAsync(CancellationToken cancellationToken);
}