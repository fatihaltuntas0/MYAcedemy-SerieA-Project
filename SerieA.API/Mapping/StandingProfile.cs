using AutoMapper;
using SerieA.API.DTOs.Standings;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class StandingProfile : Profile
    {
        public StandingProfile()
        {
            // Property names differ between Team and StandingRowDto (Name -> TeamName, Code -> TeamCode ...),
            // so AutoMapper cannot match them by convention. Each member must be mapped explicitly.
            CreateMap<Team, StandingRowDto>()
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TeamCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.TeamLogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.TeamColorOne, opt => opt.MapFrom(src => src.ColorOne))
                .ForMember(dest => dest.TeamColorTwo, opt => opt.MapFrom(src => src.ColorTwo));
        }
    }
}
