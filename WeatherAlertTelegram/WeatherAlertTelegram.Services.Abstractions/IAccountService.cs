using WeatherAlertTelegram.Domain;
using WeatherAlertTelegram.Services.Abstractions.Inputs;

namespace WeatherAlertTelegram.Services.Abstractions;

public interface IAccountService
{
    /// <summary>
    /// Register new user.
    /// </summary>
    public Task RegisterNewUserAsync(UserInput input, CancellationToken cancellationToken); 

    /// <summary>
    /// Gets list of users.
    /// </summary>
    public Task<List<UserListItem>> GetUsersAsync(CancellationToken cancellationToken);
}