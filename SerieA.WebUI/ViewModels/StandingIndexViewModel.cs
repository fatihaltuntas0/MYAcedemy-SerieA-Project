using SerieA.WebUI.DTOs.Standings;

namespace SerieA.WebUI.ViewModels
{
    public class StandingIndexViewModel
    {
        public List<ResultStandingRowDto> Standings { get; set; } = new();
        public List<int> AvailableWeeks { get; set; } = new();
        public int? SelectedWeek { get; set; }
    }
}
