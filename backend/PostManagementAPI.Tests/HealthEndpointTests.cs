using System.Net;
using FluentAssertions;

namespace PostManagementAPI.Tests
{
    /// <summary>
    /// Integration tests for Health endpoint
    /// </summary>
    public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("OK");
        }

        [Fact]
        public async Task RootEndpoint_ReturnsApiInfo()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Movie Management API");
            content.Should().Contain("status");
        }
    }
}
