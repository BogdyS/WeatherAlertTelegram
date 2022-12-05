using Microsoft.EntityFrameworkCore;
using WeatherAlertTelegram.Core;
using WeatherAlertTelegram.Core.Entities;
using WeatherAlertTelegram.Domain;
using WeatherAlertTelegram.Domain.Exceptions;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Inputs;

namespace WeatherAlertTelegram.Services;

public class AccountService : IAccountService
{
    private readonly DataContext _context;

    public AccountService(DataContext context)
    {
        _context = context;
    }

    public async Task RegisterNewUserAsync(UserInput input, CancellationToken cancellationToken)
    {
        var isUserExists = await _context.Users
            .Where(x => x.ChatId == input.Id)
            .AnyAsync(cancellationToken);

        if (isUserExists)
        {
            throw new AppException("User with given chat id already exists.");
        }

        //TODO: Validation for cities

        var user = new User
        {
            ChatId = input.Id,
            City = input.City,
            Name = input.UserName,
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<UserListItem>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return _context.Users
            .Select(x => new UserListItem
            {
                ChatId = x.ChatId,
                UserName = x.Name,
                City = x.City,
                Id = x.Id,
            })
            .ToListAsync(cancellationToken);
    }
}