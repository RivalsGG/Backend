using Moq;
using RivalsGG.BLL.Services;
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
            var expectedPlayers = new List<Player>
        {
            new Player { PlayerId = 1, PlayerName = "Player1" },
            new Player { PlayerId = 2, PlayerName = "Player2" }
        };

            _mockRepository.Setup(repo => repo.GetAllPlayersAsync())
                .ReturnsAsync(expectedPlayers);

            // Act
            var result = await _service.GetAllPlayersAsync();

            // Assert
            Assert.Equal(expectedPlayers, result);
            _mockRepository.Verify(repo => repo.GetAllPlayersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WithValidId_ReturnsPlayer()
        {
            // Arrange
            var expectedPlayer = new Player { PlayerId = 1, PlayerName = "TestPlayer" };

            _mockRepository.Setup(repo => repo.GetPlayerByIdAsync(1))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.GetPlayerByIdAsync(1);

            // Assert
            Assert.Equal(expectedPlayer, result);
            _mockRepository.Verify(repo => repo.GetPlayerByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreatePlayerAsync_CallsRepository()
        {
            // Arrange
            var player = new Player { PlayerName = "NewPlayer" };
            var expectedPlayer = new Player { PlayerId = 1, PlayerName = "NewPlayer" };

            _mockRepository.Setup(repo => repo.CreatePlayerAsync(player))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _service.CreatePlayerAsync(player);

            // Assert
            Assert.Equal(expectedPlayer, result);
            _mockRepository.Verify(repo => repo.CreatePlayerAsync(player), Times.Once);
        }
    }
}
