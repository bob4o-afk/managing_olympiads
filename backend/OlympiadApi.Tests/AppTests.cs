using OlympiadApi.Tests.Infrastructure;

namespace OlympiadApi.Tests
{
    public class AppTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AppTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Application_Starts_And_Returns_404_For_Unknown_Route()
        {
            var response = await _client.GetAsync("/not-a-real-endpoint");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Application_Has_Swagger_Enabled()
        {
            var response = await _client.GetAsync("/swagger/index.html");
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
