using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Inputs;
using WeatherAlertTelegram.Services.Abstractions.Options;

namespace WeatherAlertTelegram.Services;

public class TelegramService : ITelegramService
{
    private readonly IAccountService _accountService;
    private readonly TelegramApiOptions _telegramBotClientOptions;

    public TelegramService(
        IAccountService accountService,
        IOptions<TelegramApiOptions> telegramBotClientOptions)
    {
        _accountService = accountService;
        _telegramBotClientOptions = telegramBotClientOptions.Value;
    }

    public async Task SetWebHookAsync(string tunnel, CancellationToken cancellationToken)
    {
        var client = new TelegramBotClient(_telegramBotClientOptions.ApiKey);

        await client.SetWebhookAsync($"{tunnel}/api/bot/post", cancellationToken: cancellationToken);

        var res = await client.GetWebhookInfoAsync(cancellationToken);
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var userId = update.Message!.Chat.Id;

        var isUserExists = await _accountService.IsUserExistsAsync(userId, cancellationToken);

        if (isUserExists)
        {
            var client = new TelegramBotClient(_telegramBotClientOptions.ApiKey);

            await client.SendTextMessageAsync(chatId : userId,
                text : "Вы уже зарегестрированы и будете получать рассылку.",
                cancellationToken : cancellationToken);
        }
        else
        {
            //var userInput = new UserInput
            //{
            //    City = 
            //}
            //await _accountService.RegisterNewUserAsync(
            //    new)
        }
    }

    public async Task SendMessageAsync(
        List<long> chatIds,
        string message,
        CancellationToken cancellationToken)
    {
        var client = new TelegramBotClient(_telegramBotClientOptions.ApiKey);

        foreach (var id in chatIds)
        {
            await client.SendTextMessageAsync(chatId: id, text: message, cancellationToken: cancellationToken);
        }
        
    }
}