using AutoMapper;
using SerieA.API.DTOs.Matches;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class MatchProfile :Profile
    {
        public MatchProfile()
        {
            CreateMap<MatchUpsertDto, Match>().ReverseMap();

            CreateMap<Match, ResultMatchDto>();
        }
    }
}
