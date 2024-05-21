using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;

namespace EscapeRoomAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Player, PlayerDto>().ReverseMap();
        CreateMap<GameSession, GameSessionDto>().ReverseMap();
        CreateMap<PlayerGameSession, PlayerGameSessionDto>().ReverseMap();
    }
}