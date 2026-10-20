using AutoMapper;
using SerieA.API.DTOs;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, ResultTeamDto>().ReverseMap();
        }
    }
}
