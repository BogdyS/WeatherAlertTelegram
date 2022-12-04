using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherAlertTelegram.Core.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDataContext(this IServiceCollection service, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Local");

        service.AddScoped<Initializer>();
        service.AddDbContext<DataContext>((options) => options.UseSqlServer(connection));
        
        var serviceProvider = service.BuildServiceProvider();
        var initializer = serviceProvider.GetRequiredService<Initializer>();

        initializer.Initialize().GetAwaiter().GetResult();

        return service;
    }
}