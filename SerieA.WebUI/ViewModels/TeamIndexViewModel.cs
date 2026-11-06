using SerieA.WebUI.DTOs.Teams;

namespace SerieA.WebUI.ViewModels
{
    public class TeamIndexViewModel
    {
        public List<ResultTeamDto> Teams { get; set; } = new();
        public LeagueSummaryDto Summary { get; set; } = new();

        // To display the status of the filters on the screen:
        public string? SearchQuery { get; set; }
        public string? SelectedRegion { get; set; }
        public string? SelectedStatus { get; set; }
    }
}
