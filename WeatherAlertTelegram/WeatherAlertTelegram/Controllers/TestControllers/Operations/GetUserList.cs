using AutoMapper;
using MediatR;
using WeatherAlertTelegram.Controllers.TestControllers.Dto;
using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Controllers.TestControllers.Operations;

public class GetUserList
{
    public class GetUserListQuery : IRequest<List<UserListEntryDto>>
    {
    }

    public class Handler : IRequestHandler<GetUserListQuery, List<UserListEntryDto>>
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public Handler(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task<List<UserListEntryDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            var result = await _accountService.GetUsersAsync(cancellationToken);

            var response = _mapper.Map<List<UserListEntryDto>>(result);

            return response;
        }
    }
}