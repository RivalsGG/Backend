using Moq;
using RivalsGG.BLL.Services;
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
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _mockRepository;
        private readonly PlayerService _service;

        public PlayerServiceTests()
        {
            _mockRepository = new Mock<IPlayerRepository>();
            _service = new PlayerService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllPlayersAsync_ReturnsAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { PlayerId = 1, PlayerName = "Player1", PlayerColor = "#FF0000", PlayerAuthId = "auth1", PlayerIcon = "icon1" },
                new Player { PlayerId = 2, PlayerName = "Player2", PlayerColor = "#00FF00", PlayerAuthId = "auth2", PlayerIcon = "icon2" }
            };

            _mockRepository.Setup(repo => repo.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Player1", result.First().PlayerName);
            Assert.Equal("Player2", result.Last().PlayerName);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WithValidId_ReturnsPlayer()
        {
            // Arrange
            var player = new Player
            {
                PlayerId = 1,
                PlayerName = "TestPlayer",
                PlayerColor = "#FF0000",
                PlayerAuthId = "auth1",
                PlayerIcon = "icon1"
            };

            _mockRepository.Setup(repo => repo.GetPlayerByIdAsync(1))
                .ReturnsAsync(player);

            // Act
            var result = await _service.GetPlayerByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PlayerId);
            Assert.Equal("TestPlayer", result.PlayerName);
        }

        [Fact]
        public async Task CreatePlayerAsync_CallsRepository()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "NewPlayer",
                PlayerColor = "#FF0000"
            };

            var player = new Player
            {
                PlayerName = "NewPlayer",
                PlayerColor = "#FF0000",
                PlayerAuthId = "",
                PlayerIcon = ""
            };

            var createdPlayer = new Player
            {
                PlayerId = 1,
                PlayerName = "NewPlayer",
                PlayerColor = "#FF0000",
                PlayerAuthId = "",
                PlayerIcon = ""
            };

            _mockRepository.Setup(repo => repo.CreatePlayerAsync(It.IsAny<Player>()))
                .ReturnsAsync(createdPlayer);

            // Act
            var result = await _service.CreatePlayerAsync(playerDto);

            // Assert
            Assert.Equal(1, result.PlayerId);
            Assert.Equal("NewPlayer", result.PlayerName);
            _mockRepository.Verify(repo => repo.CreatePlayerAsync(It.Is<Player>(p =>
                p.PlayerName == playerDto.PlayerName &&
                p.PlayerColor == playerDto.PlayerColor)), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayerAsync_CallsRepository()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerId = 1,
                PlayerName = "UpdatedPlayer",
                PlayerColor = "#FF0000"
            };

            var existingPlayer = new Player
            {
                PlayerId = 1,
                PlayerName = "OldName",
                PlayerColor = "#000000",
                PlayerAuthId = "auth1",
                PlayerIcon = "icon1"
            };

            _mockRepository.Setup(repo => repo.GetPlayerByIdAsync(1))
                .ReturnsAsync(existingPlayer);

            _mockRepository.Setup(repo => repo.UpdatePlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdatePlayerAsync(playerDto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdatePlayerAsync(It.Is<Player>(p =>
                p.PlayerId == 1 &&
                p.PlayerName == "UpdatedPlayer" &&
                p.PlayerColor == "#FF0000")), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerAsync_CallsRepository()
        {
            // Arrange
            int playerId = 1;
            _mockRepository.Setup(repo => repo.DeletePlayerAsync(playerId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeletePlayerAsync(playerId);

            // Assert
            _mockRepository.Verify(repo => repo.DeletePlayerAsync(playerId), Times.Once);
        }
    }
}
