using System.Text.Json.Serialization;

namespace SerieA.WebUI.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin = 1,
        Editor = 2
    }
}
