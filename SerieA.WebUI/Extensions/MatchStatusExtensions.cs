using SerieA.WebUI.Enums;

namespace SerieA.WebUI.Extensions
{
    public static class MatchStatusExtensions
    {
        public static string ToBadgeClass(this MatchStatus status)
        {
            return status switch
            {
                MatchStatus.Finished => "active",
                MatchStatus.Live => "danger",
                MatchStatus.Postponed => "inactive",
                _ => "secondary" // NotStarted
            };
        }

        public static string ToDisplayStatus(this MatchStatus status)
        {
            return status switch
            {
                MatchStatus.Finished => "Tamamlandı",
                MatchStatus.Live => "Canlı",
                MatchStatus.Postponed => "Ertelendi",
                _ => "Oynanmadı"
            };
        }
    }
}
