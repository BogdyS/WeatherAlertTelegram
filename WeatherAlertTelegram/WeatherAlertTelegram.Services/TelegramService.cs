using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using WeatherAlertTelegram.Core;
using WeatherAlertTelegram.Core.Entities;
using WeatherAlertTelegram.Core.Enums;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Options;

namespace WeatherAlertTelegram.Services;

public class TelegramService : ITelegramService
{
    private readonly DataContext _dataContext;
    private readonly IAccountService _accountService;
    private readonly TelegramApiOptions _telegramBotClientOptions;

    public TelegramService(
        IAccountService accountService,
        IOptions<TelegramApiOptions> telegramBotClientOptions,
        DataContext dataContext)
    {
        _accountService = accountService;
        _dataContext = dataContext;
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

        var client = new TelegramBotClient(_telegramBotClientOptions.ApiKey);

        if (isUserExists)
        {
            await client.SendTextMessageAsync(chatId : userId,
                text : "Вы уже зарегестрированы и будете получать рассылку.",
                cancellationToken : cancellationToken);
        }
        else
        {
            var message = string.Empty;

            var pending = await _dataContext.PendingRegistrations
                .AsTracking()
                .Where(x => x.ChatId == userId)
                .SingleOrDefaultAsync(cancellationToken);
            if (pending == null)
            {
                _dataContext.PendingRegistrations
                    .Add(new PendingRegistrations
                    {
                        ChatId = userId,
                        Step = RegistrationSteps.FirstMessage,
                    });

                await _dataContext.SaveChangesAsync(cancellationToken);
                message = "Как я могу к вам обращаться?";
            }
            else
            {
                if (pending.Step == RegistrationSteps.FirstMessage)
                {
                    var name = update.Message.Text;
                    if (name == null)
                    {
                        return;
                    }

                    message =
                        $"Здравствуйте, {name}! Давайте завершим регистрацию: Введите город, для которого хотите " +
                        $"получать оповещения.";
                    pending.Step = RegistrationSteps.City;
                    pending.UserName = name;
                    await _dataContext.SaveChangesAsync(cancellationToken);
                }
                else if (pending.Step == RegistrationSteps.City)
                {
                    var city = update.Message.Text;
                    if (city == null)
                    {
                        return;
                    }

                    pending.Step = RegistrationSteps.Agree;
                    pending.City = city;
                    //TODO: Check city
                    await _dataContext.SaveChangesAsync(cancellationToken);
                    message =
                        "Давайте проверим информацию:\n" +
                        $"Ваше имя: {pending.UserName}\n" +
                        $"Город: {pending.City}.\n";

                    await client.SendTextMessageAsync(chatId: userId,
                        text: message,
                        cancellationToken: cancellationToken);
                    message = "Все верно? Ответьте 'да' или 'нет'";
                }
                else if (pending.Step == RegistrationSteps.Agree)
                {
                    var response = update.Message.Text;
                    if (response == null)
                    {
                        return;
                    }

                    response = response.ToLower().Trim();
                    if (response == "да")
                    {
                        _dataContext.Users.Add(new Core.Entities.User
                        {
                            City = pending.City,
                            ChatId = pending.ChatId,
                            Name = pending.UserName,
                        });

                        _dataContext.PendingRegistrations.Remove(pending);
                        await _dataContext.SaveChangesAsync(cancellationToken);

                        message = "Готово! Теперь вы подключены к рассылке.";
                    }
                    else if (response == "нет")
                    {
                        pending.Step = RegistrationSteps.FirstMessage;
                        await _dataContext.SaveChangesAsync(cancellationToken);

                        message = "Давайте попробуем сначала.";

                        await client.SendTextMessageAsync(chatId: userId,
                            text: message,
                            cancellationToken: cancellationToken);

                        message = "Как я могу к вам обращаться?";
                    }
                    else
                    {
                        return;
                    }
                }
            }
            await client.SendTextMessageAsync(chatId: userId,
                text: message,
                cancellationToken: cancellationToken);
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