
using System.Collections;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace OlympiadApi.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void ValidateConnectionString_Throws_WhenConnectionStringIsNull()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Program.ValidateConnectionString(null));
            Assert.Contains("connection string 'DefaultConnection' was not found", ex.Message);
        }

        [Fact]
        public void ValidateConnectionString_Throws_WhenConnectionStringIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Program.ValidateConnectionString(""));
            Assert.Contains("connection string 'DefaultConnection' was not found", ex.Message);
        }

        [Fact]
        public void ValidateConnectionString_DoesNotThrow_WhenConnectionStringIsValid()
        {
            Program.ValidateConnectionString("Server=localhost;Database=test;Uid=root;Pwd=root;");
        }

        [Theory]
        [InlineData("JWT_SECRET_KEY")]
        [InlineData("JWT_ISSUER")]
        [InlineData("JWT_AUDIENCE")]
        public void ValidateJwtConfiguration_Throws_WhenKeyIsMissing(string missingKey)
        {
            var configValues = new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "test_secret",
                ["JWT_ISSUER"] = "test_issuer",
                ["JWT_AUDIENCE"] = "test_audience"
            };

            configValues[missingKey] = null;

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var ex = Assert.Throws<InvalidOperationException>(() => Program.ValidateJwtConfiguration(config));
            Assert.Contains(missingKey, ex.Message);
        }

        [Fact]
        public void ValidateJwtConfiguration_DoesNotThrow_WhenAllKeysPresent()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_SECRET_KEY"] = "test_secret",
                    ["JWT_ISSUER"] = "test_issuer",
                    ["JWT_AUDIENCE"] = "test_audience"
                })
                .Build();

            Program.ValidateJwtConfiguration(config);
        }
        
        [Fact]
        public void MapEnvironmentVariablesToConfiguration_CopiesAllValues()
        {
            var config = new ConfigurationManager();
            var env = new Hashtable
            {
                { "KEY1", "value1" },
                { "KEY2", null }
            };

            Program.MapEnvironmentVariablesToConfiguration(config, env);

            Assert.Equal("value1", config["KEY1"]);
            Assert.Null(config["KEY2"]);
        }
    }
}
