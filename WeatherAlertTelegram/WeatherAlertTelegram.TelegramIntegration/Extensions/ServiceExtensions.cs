using WeatherAlertTelegram.Core.Extensions;
using WeatherAlertTelegram.Services;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Options;

namespace WeatherAlertTelegram.TelegramIntegration.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<ITelegramService, TelegramService>();
        service.AddScoped<IAccountService, AccountService>();
        service.AddDataContext(configuration);
        service.Configure<TelegramApiOptions>(configuration.GetSection("TelegramConfiguration"));

        return service;
    }
}