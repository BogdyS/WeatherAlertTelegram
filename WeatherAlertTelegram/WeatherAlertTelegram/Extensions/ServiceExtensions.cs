using WeatherAlertTelegram.Core.Extensions;

namespace WeatherAlertTelegram.Extensions;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder app)
    {
        app.Services.AddDataContext(app.Configuration);

        return app;
    }
}