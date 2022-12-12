using Telegram.Bot.Types;

namespace WeatherAlertTelegram.Services.Abstractions;

public interface ITelegramService
{
    Task HandleAsync(Update update, CancellationToken cancellationToken);

    Task SetWebHookAsync(string tunnel, CancellationToken cancellationToken);

    Task SendMessageAsync(
        List<long> chatIds,
        string message,
        CancellationToken cancellationToken);
}