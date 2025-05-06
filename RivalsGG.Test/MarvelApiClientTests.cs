using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using RivalsGG.BLL.Services;
using RivalsGG.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RivalsGG.Test
{
    public class MarvelApiClientTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public MarvelApiClientTests()
        {
            // Mock API key
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["MarvelRivalsApi:ApiKey"]).Returns("test-api-key");

            // Mock HTTP 
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://marvelrivalsapi.com/api/v1/")
            };
        }

        [Fact]
        public async Task GetHeroesAsync_ReturnsHeroes_WhenApiCallSucceeds()
        {
            // Arrange
            var heroes = new List<MarvelHero>
            {
                new MarvelHero { Id = "1", Name = "Iron Man", ImageUrl = "/Ironman.png", Role = "Attacker"},
                new MarvelHero { Id = "2", Name = "Spider-Man", ImageUrl = "/Spider-Man.png", Role = "Attacker" }
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(heroes))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new MarvelApiClient(_httpClient, _mockConfig.Object);

            // Act
            var result = await client.GetHeroesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, h => h.Name == "Iron Man");
            Assert.Contains(result, h => h.Name == "Spider-Man");
        }

        [Fact]
        public async Task GetHeroByIdAsync_ReturnsHero_WhenApiCallSucceeds()
        {
            // Arrange
            var hero = new MarvelHero { Id = "1", Name = "Thor", ImageUrl = "/Thor.png", Role = "Tank" };
            var jsonResponse = JsonSerializer.Serialize(hero);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new MarvelApiClient(_httpClient, _mockConfig.Object);

            // Act
            var result = await client.GetHeroByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Thor", result.Name);
            Assert.Equal("/Thor.png", result.ImageUrl);
            Assert.Equal("Tank", result.Role);
        }

        [Fact]
        public async Task GetHeroesAsync_HandlesEmptyResponse()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new MarvelApiClient(_httpClient, _mockConfig.Object);

            // Act
            var result = await client.GetHeroesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ApiClient_IncludesApiKeyInHeader()
        {
            // Arrange
            var heroes = new List<MarvelHero>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(heroes))
            };

            HttpRequestMessage capturedRequest = null;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(response);

            // Act
            var client = new MarvelApiClient(_httpClient, _mockConfig.Object);
            await client.GetHeroesAsync();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest.Headers.Contains("x-api-key"));
            capturedRequest.Headers.TryGetValues("x-api-key", out var values);
            Assert.Contains("test-api-key", values);
        }
    }
}
   
