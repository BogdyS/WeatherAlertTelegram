using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using WeatherAlertTelegram.Core.Entities;

namespace WeatherAlertTelegram.Core;

public class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
    : base(options)
    {
    }

    public virtual DbSet<User> Users => Set<User>();

    public virtual DbSet<PendingRegistrations> PendingRegistrations => Set<PendingRegistrations>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(
            x => x.MigrationsHistoryTable(
                HistoryRepository.DefaultTableName)
        );
    }
}