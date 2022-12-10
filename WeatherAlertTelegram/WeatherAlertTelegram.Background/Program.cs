using Hangfire;
using Hangfire.SqlServer;
using WeatherAlertTelegram.Background.Extensions;

namespace WeatherAlertTelegram.Background
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var options = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true,
            };

            var connectionString = builder.Configuration.GetConnectionString("HangfireConnection");

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString, options);

            builder.Services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings());
            builder.Services.RegisterServices(builder.Configuration);

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            app.UseHangfireDashboard("/hangfire");

            app.Run();
        }
    }
}