using AutoMapper;
using SerieA.API.DTOs.Matches.Events;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class MatchEventProfile : Profile
    {
        public MatchEventProfile()
        {
            CreateMap<CreateCardDto, MatchCard>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerName.Trim()))
                .ForMember(dest => dest.Minute, opt => opt.MapFrom(src => src.Minute.Trim()));

            CreateMap<CreateGoalDto, MatchGoal>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerName.Trim()))
                .ForMember(dest => dest.Minute, opt => opt.MapFrom(src => src.Minute.Trim()));

            CreateMap<CreateSubstitutionDto, Substitution>()
                .ForMember(dest => dest.PlayerIn, opt => opt.MapFrom(src => src.PlayerIn.Trim()))
                .ForMember(dest => dest.PlayerOut, opt => opt.MapFrom(src => src.PlayerOut.Trim()))
                .ForMember(dest => dest.Minute, opt => opt.MapFrom(src => src.Minute.Trim()));
        }
    }
}

