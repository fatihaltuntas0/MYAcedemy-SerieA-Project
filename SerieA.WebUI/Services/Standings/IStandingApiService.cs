using SerieA.WebUI.DTOs.Standings;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Standings
{
    public interface IStandingApiService
    {
        Task<ApiResponse<List<ResultStandingRowDto>>> GetStandingAsync(int? upToWeek = null, CancellationToken cancellationToken = default);
    }
}
