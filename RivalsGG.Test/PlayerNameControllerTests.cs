using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RivalsGG.API.Controllers;
using RivalsGG.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace RivalsGG.Test
{   
    public class PlayerNameControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<PlayerNameController>> _mockLogger;

        public PlayerNameControllerTests()
        {
            // Mock API key
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["MarvelRivalsApi:ApiKey"]).Returns("test-api-key");

            // Mock HTTP
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://marvelrivalsapi.com/")
            };

            // Mock logger
            _mockLogger = new Mock<ILogger<PlayerNameController>>();
        }

        [Fact]
        public async Task GetPlayerNameByUid_WithGoodUid_ReturnsOkWithPlayerName()
        {
            // Arrange
            var uid = "gg";
            var expectedName = "TestPlayer";

            var playerResponse = new { uid = uid, name = expectedName };
            var jsonResponse = JsonSerializer.Serialize(playerResponse);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"player/{uid}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Create client and controller
            var marvelApiClient = new MarvelApiClient(_httpClient, _mockConfig.Object);
            var controller = new PlayerNameController(marvelApiClient, _mockLogger.Object);

            // Act
            var result = await controller.GetPlayerNameByUid(uid);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseObj = okResult.Value;

            // Access properties using reflection
            var uidProperty = responseObj.GetType().GetProperty("uid").GetValue(responseObj);
            var usernameProperty = responseObj.GetType().GetProperty("username").GetValue(responseObj);

            Assert.Equal(uid, uidProperty);
            Assert.Equal(expectedName, usernameProperty);
        }
        [Fact]
        public async Task GetPlayerNameByUid_WithBadUid_ReturnsBadRequest()
        {
            // Arrange
            var uid = "test123"; 

            var marvelApiClient = new MarvelApiClient(_httpClient, _mockConfig.Object);
            var controller = new PlayerNameController(marvelApiClient, _mockLogger.Object);

            // Act
            var result = await controller.GetPlayerNameByUid(uid);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("UID can only be number", badRequestResult.Value);
        }
    }
}
