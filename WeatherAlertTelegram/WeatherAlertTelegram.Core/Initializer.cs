using Microsoft.EntityFrameworkCore;

namespace WeatherAlertTelegram.Core;

public class Initializer
{
    private readonly DataContext _context;

    public Initializer(DataContext context)
    {
        _context = context;
    }

    public async Task Initialize()
    {
        await _context.Database.MigrateAsync();
    }
}