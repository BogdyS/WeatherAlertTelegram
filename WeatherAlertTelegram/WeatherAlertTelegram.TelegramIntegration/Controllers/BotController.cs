using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.TelegramIntegration.Controllers;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly ITelegramService _telegramService;

    public BotController(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    [HttpPost("post")]
    public async Task<IActionResult> PostAsync([FromBody] Update update, CancellationToken cancellationToken)
    {
        await _telegramService.HandleAsync(update, cancellationToken);

        return Ok();
    }

    [HttpPost("set-webhook")]
    public async Task<IActionResult> SetWebhookAsync(CancellationToken cancellationToken)
    {
        await _telegramService.SetWebHookAsync(cancellationToken);
        return Ok();
    }
}