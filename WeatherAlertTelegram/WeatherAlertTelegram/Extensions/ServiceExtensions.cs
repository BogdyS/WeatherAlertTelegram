using MediatR;
using WeatherAlertTelegram.Core.Extensions;
using WeatherAlertTelegram.Services;
using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Extensions;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterDatabase(this WebApplicationBuilder app)
    {
        app.Services.AddDataContext(app.Configuration);

        return app;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
#if DEBUG
        service.AddScoped<IAccountService, AccountService>();
#endif
        service.AddMediatR(typeof(AssemblyMarker).Assembly);
        service.AddAutoMapper(typeof(AssemblyMarker).Assembly);
        return service;
    }
}