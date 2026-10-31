using System.Text.Json.Serialization;

namespace SerieA.WebUI.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MatchStatus
    {
        NotStarted = 0,
        Live = 1,
        Finished = 2,
        Postponed = 3
    }
}
