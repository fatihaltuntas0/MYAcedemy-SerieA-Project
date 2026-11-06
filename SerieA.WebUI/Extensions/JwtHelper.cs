using System.Text.Json;

namespace SerieA.WebUI.Extensions
{
    public static class JwtHelper
    {
        public static bool IsTokenExpired(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return true;

            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2) return true;

                var payload = parts[1];
                payload = payload.Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                using var doc = JsonDocument.Parse(jsonBytes);
                if (doc.RootElement.TryGetProperty("exp", out var expProp))
                {
                    var expSeconds = expProp.GetInt64();
                    var expirationUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                    return expirationUtc <= DateTime.UtcNow;
                }
            }
            catch
            {
                return true;
            }

            return true;
        }
    }
}

