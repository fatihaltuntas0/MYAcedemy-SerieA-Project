using AutoMapper;
using SerieA.API.DTOs.Standings;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class StandingProfile : Profile
    {
        public StandingProfile()
        {
            CreateMap<Team, StandingRowDto>()
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Name));

        }
    }
}
