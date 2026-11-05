using AutoMapper;
using SerieA.API.DTOs.Matches;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            CreateMap<MatchUpsertDto, Match>().ReverseMap();

            CreateMap<Match, ResultMatchDto>();

            CreateMap<MatchGoal, MatchGoalDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src =>
                    src.Team != null ? src.Team.Name :
                    src.Match != null ? (src.TeamId == src.Match.HomeTeamId ? src.Match.HomeTeam.Name : src.Match.AwayTeam.Name) :
                    string.Empty));

            CreateMap<MatchCard, MatchCardDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src =>
                    src.Team != null ? src.Team.Name :
                    src.Match != null ? (src.TeamId == src.Match.HomeTeamId ? src.Match.HomeTeam.Name : src.Match.AwayTeam.Name) :
                    string.Empty));

            CreateMap<Substitution, SubstitutionDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src =>
                    src.Team != null ? src.Team.Name :
                    src.Match != null ? (src.TeamId == src.Match.HomeTeamId ? src.Match.HomeTeam.Name : src.Match.AwayTeam.Name) :
                    string.Empty));

            CreateMap<Match, ResultMatchDetailDto>()
                .IncludeBase<Match, ResultMatchDto>()
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.MatchGoals))
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.MatchCards))
                .ForMember(dest => dest.Substitutions, opt => opt.MapFrom(src => src.Substitutions));
        }
    }
}
