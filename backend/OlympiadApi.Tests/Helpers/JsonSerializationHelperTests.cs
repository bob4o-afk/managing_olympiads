using System.Text.Json;
using OlympiadApi.Helpers;

namespace OlympiadApi.Tests.Helpers
{
    public class JsonSerializationHelperTests
    {
        [Fact]
        public void SerializeToJson_ReturnsNull_WhenDictionaryIsNull()
        {
            Dictionary<string, object>? input = null;

            var result = JsonSerializationHelper.SerializeToJson(input);

            Assert.Null(result);
        }

        [Fact]
        public void SerializeToJson_ReturnsJsonString_WhenDictionaryIsValid()
        {
            var input = new Dictionary<string, object>
            {
                { "key", "value" },
                { "number", 123 }
            };

            var result = JsonSerializationHelper.SerializeToJson(input);

            Assert.NotNull(result);
            
            var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(result!);
            Assert.Equal("value", parsed!["key"].ToString());
            Assert.Equal("123", parsed["number"].ToString());
        }

        [Fact]
        public void DeserializeFromJson_ReturnsNull_WhenJsonIsNull()
        {
            string? json = null;

            var result = JsonSerializationHelper.DeserializeFromJson(json);

            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromJson_ReturnsDictionary_WhenJsonIsValid()
        {
            var json = "{\"key\":\"value\",\"number\":123}";

            var result = JsonSerializationHelper.DeserializeFromJson(json);

            Assert.NotNull(result);
            Assert.Equal("value", result!["key"].ToString());
            Assert.Equal("123", result["number"].ToString());
        }
    }
}
