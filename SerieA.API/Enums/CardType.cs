using System.Text.Json.Serialization;

namespace SerieA.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CardType
    {
        Yellow = 1,
        Red = 2
    }
}
