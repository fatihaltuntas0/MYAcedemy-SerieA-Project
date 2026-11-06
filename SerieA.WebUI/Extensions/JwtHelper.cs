using System.Text.Json;

namespace SerieA.WebUI.Extensions
{
    public static class JwtHelper
    {

        public static DateTimeOffset? GetExpiration(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2) return null;

                var payload = parts[1].Replace('-', '+').Replace('_', '/');
                payload = (payload.Length % 4) switch { 2 => payload + "==", 3 => payload + "=", _ => payload };

                using var doc = JsonDocument.Parse(Convert.FromBase64String(payload));
                if (doc.RootElement.TryGetProperty("exp", out var exp))
                    return DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64());
            }
            catch { /* invalid token */ }
            return null;
        }

        public static bool IsTokenExpired(string? token) => GetExpiration(token) is not { } exp || exp <= DateTimeOffset.UtcNow;
    }
}

