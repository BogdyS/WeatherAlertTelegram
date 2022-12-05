using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WeatherAlertTelegram.Services.Abstractions;
using WeatherAlertTelegram.Services.Abstractions.Inputs;

namespace WeatherAlertTelegram.Controllers.TestControllers.Operations;

public class RegisterUser 
{
    public class RegisterUserCommand : IRequest<bool>
    {
        /// <summary>
        /// Data for user registration.
        /// </summary>
        [Required]
        [FromBody]
        public UserInput Input { get; set; }
    }

    public class Handler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IAccountService _accountService;

        public Handler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            await _accountService.RegisterNewUserAsync(request.Input, cancellationToken);

            return true;
        }
    }
}