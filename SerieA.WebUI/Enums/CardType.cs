 using System.Text.Json.Serialization;

namespace SerieA.WebUI.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CardType
    {
        Yellow = 1,
        Red = 2
    }
}
