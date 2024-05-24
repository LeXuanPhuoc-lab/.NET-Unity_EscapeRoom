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
        CreateMap<PlayerGameAnswer, PlayerGameAnswerDto>().ReverseMap();
        CreateMap<Question, QuestionDto>().ReverseMap();
        CreateMap<QuestionAnswer, QuestionAnswerDto>().ReverseMap();
        CreateMap<Leaderboard, LeaderboardDto>().ReverseMap();
    }
}