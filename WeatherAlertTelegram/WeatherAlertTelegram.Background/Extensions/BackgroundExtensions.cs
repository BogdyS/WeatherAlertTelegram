using Hangfire;
using Microsoft.EntityFrameworkCore;
using WeatherAlertTelegram.Background.Workers;
using WeatherAlertTelegram.Core;
using WeatherAlertTelegram.Core.Extensions;
using WeatherAlertTelegram.Services;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Options;

namespace WeatherAlertTelegram.Background.Extensions;

public static class BackgroundExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<WeatherApiOptions>(configuration.GetSection("WeatherApiOptions"));
        service.AddHttpClient<IWeatherService, WeatherService>();
        service.AddScoped<IAccountService, AccountService>();

        service.AddDataContext(configuration);
        RecurringJobs();
        return service;
    }

    public static void RecurringJobs()
    {
        RecurringJob.AddOrUpdate<WeatherApiWatcher>(
            WeatherApiWatcher.TaskName,
            x => x.ExecuteAsync(),
            Cron.Hourly);
    }
}