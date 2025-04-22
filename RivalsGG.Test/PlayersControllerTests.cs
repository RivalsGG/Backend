using Microsoft.AspNetCore.Mvc;
using Moq;
using RivalsGG.API.Controllers;
using RivalsGG.Core.DTOs;
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
            var players = new List<PlayerDTO>
            {
                new PlayerDTO { PlayerId = 1, PlayerName = "Player1", PlayerColor = "#FF0000" },
                new PlayerDTO { PlayerId = 2, PlayerName = "Player2", PlayerColor = "#00FF00" }
            };

            _mockService.Setup(service => service.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _controller.GetPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlayers = Assert.IsAssignableFrom<IEnumerable<PlayerDTO>>(okResult.Value);
            Assert.Equal(2, returnedPlayers.Count());
        }

        [Fact]
        public async Task GetPlayer_WithValidId_ReturnsOkWithPlayer()
        {
            // Arrange
            var player = new PlayerDTO { PlayerId = 1, PlayerName = "TestPlayer", PlayerColor = "#FF0000" };

            _mockService.Setup(service => service.GetPlayerByIdAsync(1))
                .ReturnsAsync(player);

            // Act
            var result = await _controller.GetPlayer(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlayer = Assert.IsType<PlayerDTO>(okResult.Value);
            Assert.Equal(1, returnedPlayer.PlayerId);
        }

        [Fact]
        public async Task GetPlayer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetPlayerByIdAsync(999))
                .ReturnsAsync((PlayerDTO)null);

            // Act
            var result = await _controller.GetPlayer(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePlayer_ReturnsCreatedAtAction()
        {
            // Arrange
            var playerToCreate = new PlayerDTO
            {
                PlayerName = "NewPlayer",
                PlayerColor = "#FF0000"
            };

            var createdPlayer = new PlayerDTO
            {
                PlayerId = 1,
                PlayerName = "NewPlayer",
                PlayerColor = "#FF0000"
            };

            _mockService.Setup(service => service.CreatePlayerAsync(It.IsAny<PlayerDTO>()))
                .ReturnsAsync(createdPlayer);

            // Act
            var result = await _controller.CreatePlayer(playerToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PlayerController.GetPlayer), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);

            var returnedPlayer = Assert.IsType<PlayerDTO>(createdAtActionResult.Value);
            Assert.Equal(1, returnedPlayer.PlayerId);
        }

        [Fact]
        public async Task UpdatePlayer_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerId = 1,
                PlayerName = "UpdatedPlayer",
                PlayerColor = "#FF0000"
            };

            _mockService.Setup(service => service.UpdatePlayerAsync(It.IsAny<PlayerDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePlayer(1, playerDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePlayer_ReturnsNoContent()
        {
            // Arrange
            int playerId = 1;
            _mockService.Setup(service => service.DeletePlayerAsync(playerId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePlayer(playerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
