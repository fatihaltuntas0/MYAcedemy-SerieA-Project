using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.DTOs.Teams;

namespace SerieA.WebUI.ViewModels
{
    public class MatchIndexViewModel
    {
        public List<ResultMatchDto> Matches { get; set; } = new();
        public List<ResultTeamDto> Teams { get; set; } = new();

        // Filtre durumları
        public int? SelectedWeek { get; set; }
        public string? SearchTeam { get; set; }
        public string? SelectedStatus { get; set; }
    }
}
