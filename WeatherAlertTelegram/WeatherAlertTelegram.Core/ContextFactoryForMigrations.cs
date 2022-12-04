using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WeatherAlertTelegram.Core;

public class ContextFactoryForMigrations : IDesignTimeDbContextFactory<DataContext>
{
    public const string ConnectionString = "data source=localhost;initial catalog=WeatherAlertTelegram;trusted_connection=true";
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        optionsBuilder.UseSqlServer(ConnectionString);

        return new DataContext(optionsBuilder.Options);
    }
}