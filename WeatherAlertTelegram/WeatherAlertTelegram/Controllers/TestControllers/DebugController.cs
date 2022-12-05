using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherAlertTelegram.Controllers.TestControllers.Operations;

namespace WeatherAlertTelegram.Controllers.TestControllers;

[Route("debug")]
public class DebugController : ControllerBase
{
    private readonly IMediator _mediator;

    public DebugController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/users/register")]
    public async Task<IActionResult> RegisterNewUserAsync(
        [FromQuery] RegisterUser.RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(new {Success = true});
    }

    [HttpGet("/users/list")]
    public async Task<IActionResult> GetUserListAsync(CancellationToken cancellationToken)
    {
        var request = new GetUserList.GetUserListQuery();

        var result = await _mediator.Send(request, cancellationToken);

        IActionResult response = result?.Count > 0 ? Ok(result) : NoContent();

        return response;
    }
}