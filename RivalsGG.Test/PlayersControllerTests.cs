using Microsoft.AspNetCore.Mvc;
using Moq;
using RivalsGG.API.Controllers;
using RivalsGG.Core.Interfaces;
using RivalsGG.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Test
{
    public class PlayersControllerTests
    {
        private readonly Mock<IPlayerService> _mockService;
        private readonly PlayerController _controller;

        public PlayersControllerTests()
        {
            _mockService = new Mock<IPlayerService>();
            _controller = new PlayerController(_mockService.Object);
        }

        [Fact]
        public async Task GetPlayers_ReturnsOkWithPlayers()
        {
            // Arrange
            var players = new List<Player>
        {
            new Player { PlayerId = 1, PlayerName = "Player1" },
            new Player { PlayerId = 2, PlayerName = "Player2" }
        };

            _mockService.Setup(service => service.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _controller.GetPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlayers = Assert.IsAssignableFrom<IEnumerable<Player>>(okResult.Value);
            Assert.Equal(2, returnedPlayers.Count());
        }

        [Fact]
        public async Task GetPlayer_WithValidId_ReturnsOkWithPlayer()
        {
            // Arrange
            var player = new Player { PlayerId = 1, PlayerName = "TestPlayer" };

            _mockService.Setup(service => service.GetPlayerByIdAsync(1))
                .ReturnsAsync(player);

            // Act
            var result = await _controller.GetPlayer(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(1, returnedPlayer.PlayerId);
        }

        [Fact]
        public async Task GetPlayer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetPlayerByIdAsync(999))
                .ReturnsAsync((Player)null);

            // Act
            var result = await _controller.GetPlayer(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePlayer_ReturnsCreatedAtAction()
        {
            // Arrange
            var playerToCreate = new Player { PlayerName = "NewPlayer" };
            var createdPlayer = new Player { PlayerId = 1, PlayerName = "NewPlayer" };

            _mockService.Setup(service => service.CreatePlayerAsync(playerToCreate))
                .ReturnsAsync(createdPlayer);

            // Act
            var result = await _controller.CreatePlayer(playerToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PlayerController.GetPlayer), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);

            var returnedPlayer = Assert.IsType<Player>(createdAtActionResult.Value);
            Assert.Equal(1, returnedPlayer.PlayerId);
        }
    }
}
