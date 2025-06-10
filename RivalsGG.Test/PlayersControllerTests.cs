using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using RivalsGG.API.Controllers;
using RivalsGG.API.Hubs;
using RivalsGG.Core.DTOs;
using RivalsGG.Core.Interfaces;

namespace RivalsGG.Tests.Controllers
{
    public class PlayerControllerTests
    {
        private readonly Mock<IPlayerService> _mockPlayerService;
        private readonly Mock<IHubContext<Playerhub>> _mockHubContext;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly PlayerController _controller;

        public PlayerControllerTests()
        {
            _mockPlayerService = new Mock<IPlayerService>();
            _mockHubContext = new Mock<IHubContext<Playerhub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            // Setup for signalR
            _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
            _mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

            _controller = new PlayerController(_mockPlayerService.Object, _mockHubContext.Object);
        }

        [Fact]
        public async Task GetPlayers_ReturnsOkResult_WithListOfPlayers()
        {
            // Arrange
            var players = new List<PlayerDTO>
            {
                new PlayerDTO { PlayerId = 1, PlayerName = "Player 1" },
                new PlayerDTO { PlayerId = 2, PlayerName = "Player 2" }
            };
            _mockPlayerService.Setup(s => s.GetAllPlayersAsync()).ReturnsAsync(players);

            // Act
            var result = await _controller.GetPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlayers = Assert.IsAssignableFrom<IEnumerable<PlayerDTO>>(okResult.Value);
            Assert.Equal(2, returnedPlayers.Count());
        }

        [Fact]
        public async Task GetPlayer_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var playerId = 1;
            var player = new PlayerDTO { PlayerId = playerId, PlayerName = "Test Player" };
            _mockPlayerService.Setup(s => s.GetPlayerByIdAsync(playerId)).ReturnsAsync(player);

            // Act
            var result = await _controller.GetPlayer(playerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlayer = Assert.IsType<PlayerDTO>(okResult.Value);
            Assert.Equal(playerId, returnedPlayer.PlayerId);
        }

        [Fact]
        public async Task GetPlayer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var playerId = 999;
            _mockPlayerService.Setup(s => s.GetPlayerByIdAsync(playerId)).ReturnsAsync((PlayerDTO?)null);

            // Act
            var result = await _controller.GetPlayer(playerId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePlayer_ValidPlayer_ReturnsCreatedResult_AndBroadcastsSignalR()
        {
            // Arrange
            var newPlayer = new PlayerDTO { PlayerId = 1, PlayerName = "New Player" };
            _mockPlayerService.Setup(s => s.CreatePlayerAsync(It.IsAny<PlayerDTO>())).ReturnsAsync(newPlayer);

            // Act
            var result = await _controller.CreatePlayer(newPlayer);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetPlayer", createdResult.ActionName);
            Assert.Equal(newPlayer.PlayerId, ((PlayerDTO)createdResult.Value!).PlayerId);

            // Verify SignalR started
            _mockClientProxy.Verify(
                c => c.SendCoreAsync("PlayerCreated", It.Is<object[]>(o => o.Length == 1), default),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePlayer_ValidPlayer_ReturnsNoContent_AndBroadcastsSignalR()
        {
            // Arrange
            var playerId = 1;
            var playerDto = new PlayerDTO { PlayerId = playerId, PlayerName = "Updated Player" };
            _mockPlayerService.Setup(s => s.UpdatePlayerAsync(playerDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePlayer(playerId, playerDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify service was called
            _mockPlayerService.Verify(s => s.UpdatePlayerAsync(playerDto), Times.Once);

            // Verify SignalR started
            _mockClientProxy.Verify(
                c => c.SendCoreAsync("PlayerUpdated", It.Is<object[]>(o => o.Length == 1), default),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePlayer_PlayerNotFound_ReturnsNotFound()
        {
            // Arrange
            var playerId = 999;
            var playerDto = new PlayerDTO { PlayerId = playerId, PlayerName = "Non-existent Player" };
            _mockPlayerService.Setup(s => s.UpdatePlayerAsync(playerDto)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdatePlayer(playerId, playerDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePlayer_ValidId_ReturnsNoContent_AndBroadcastsSignalR()
        {
            // Arrange
            var playerId = 1;
            var player = new PlayerDTO { PlayerId = playerId, PlayerName = "Player to Delete" };
            _mockPlayerService.Setup(s => s.GetPlayerByIdAsync(playerId)).ReturnsAsync(player);
            _mockPlayerService.Setup(s => s.DeletePlayerAsync(playerId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePlayer(playerId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify service was called
            _mockPlayerService.Verify(s => s.DeletePlayerAsync(playerId), Times.Once);

            // Verify SignalR started
            _mockClientProxy.Verify(
                c => c.SendCoreAsync("PlayerDeleted", It.Is<object[]>(o => o.Length == 1), default),
                Times.Once);
        }

        [Fact]
        public async Task DeletePlayer_PlayerNotFound_ReturnsNotFound()
        {
            // Arrange
            var playerId = 999;
            _mockPlayerService.Setup(s => s.DeletePlayerAsync(playerId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeletePlayer(playerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}