using AutoMapper;
using WeatherAlertTelegram.Controllers.TestControllers.Dto;
using WeatherAlertTelegram.Domain;

namespace WeatherAlertTelegram.Controllers.TestControllers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserListItem, UserListEntryDto>()
            .ReverseMap();
    }
}