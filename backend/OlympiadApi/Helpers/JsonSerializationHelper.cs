using System.Text.Json;

namespace OlympiadApi.Helpers
{
    public static class JsonSerializationHelper
    {
        public static string? SerializeToJson(Dictionary<string, object>? dictionary)
        {
            return dictionary == null ? null : JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = false });
        }

        public static Dictionary<string, object>? DeserializeFromJson(string? json)
        {
            return json == null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}