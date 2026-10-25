namespace SerieA.WebUI.Extensions
{
    public static class MatchStatusExtensions
    {
        public static string ToBadgeClass(this string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "secondary";

            return status.Trim().ToLowerInvariant() switch
            {
                "tamamlandı" or "finished" or "1" => "active",
                "canlı" or "live" => "danger",
                "ertelendi" or "postponed" => "inactive",
                _  => "secondary" // NotStarted
            };
        }

        public static string ToDisplayStatus(this string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Oynanmadı";

            return status.Trim().ToLowerInvariant() switch
            {
                "tamamlandı" or "finished" or "1" => "Tamamlandı",
                "canlı" or "live" => "Canlı",
                "ertelendi" or "postponed" => "Ertelendi",
                _ => "Oynanmadı"
            };
        }
    }
}
