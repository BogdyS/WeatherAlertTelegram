using WeatherAlertTelegram.Domain;
using WeatherAlertTelegram.Services.Abstractions.Inputs;

namespace WeatherAlertTelegram.Services.Abstractions;

public interface IAccountService
{
    /// <summary>
    /// Register new user.
    /// </summary>
    Task RegisterNewUserAsync(UserInput input, CancellationToken cancellationToken); 

    /// <summary>
    /// Gets list of users.
    /// </summary>
    Task<List<UserListItem>> GetUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Checks is user exists.
    /// </summary>
    Task<bool> IsUserExistsAsync(long chatId, CancellationToken cancellationToken);
}