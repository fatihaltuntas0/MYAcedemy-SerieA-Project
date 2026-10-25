using SerieA.API.Common;
using SerieA.API.DTOs.Standings;

namespace SerieA.API.Services.Standings
{
    public interface IStandingService
    {
        Task<ApiResponse<List<StandingRowDto>>> CalculateStandingAsync();
    }
}
