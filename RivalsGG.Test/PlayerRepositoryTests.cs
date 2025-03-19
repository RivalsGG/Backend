using Microsoft.EntityFrameworkCore;
using RivalsGG.Core;
using RivalsGG.DAL;
using RivalsGG.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Test
{
    public class PlayerRepositoryTests
    {
        private DbContextOptions<PlayerDbContext> _options;

        [Fact]
        public async Task GetAllPlayersAsync_ReturnsAllPlayers()
        {
            // Arrange
            await using var context = new PlayerDbContext(_options);
            var repository = new PlayerRepository(context);
            await SeedTestData(context);

            // Act
            var players = await repository.GetAllPlayersAsync();

            // Assert
            Assert.Equal(1, players.Count()); 
        }
        [Fact]
        public async Task GetPlayerByIdAsync_WithValidId_ReturnsPlayer()
        {
            // Arrange
            await using var context = new PlayerDbContext(_options);
            var repository = new PlayerRepository(context);
            await SeedTestData(context);

            // Act
            var player = await repository.GetPlayerByIdAsync(1);

            // Assert
            Assert.NotNull(player);
            Assert.Equal("TestPlayer1", player.PlayerName);
        }

        [Fact]
        public async Task CreatePlayerAsync_AddsPlayerToDatabase()
        {
            // Arrange
            await using var context = new PlayerDbContext(_options);
            var repository = new PlayerRepository(context);

            var newPlayer = new Player
            {
                PlayerName = "NewPlayer",
                PlayerAuthId = "authnew",
                PlayerIcon = "icon.png",
                PlayerColor = "#FFFFFF"
            };

            // Act
            var result = await repository.CreatePlayerAsync(newPlayer);

            // Assert
            Assert.NotEqual(0, result.PlayerId); 

            // Verify the player is in the database
            var playerInDb = await context.Players.FindAsync(result.PlayerId);
            Assert.NotNull(playerInDb);
            Assert.Equal("NewPlayer", playerInDb.PlayerName);
        }

        private async Task SeedTestData(PlayerDbContext context)
        {
            context.Players.AddRange(
                new Player { PlayerId = 1, PlayerName = "TestPlayer1", PlayerAuthId = "auth1", PlayerIcon = "icon1.png", PlayerColor = "#FF0000" },
                new Player { PlayerId = 2, PlayerName = "TestPlayer2", PlayerAuthId = "auth2", PlayerIcon = "icon2.png", PlayerColor = "#00FF00" },
                new Player { PlayerId = 3, PlayerName = "TestPlayer3", PlayerAuthId = "auth3", PlayerIcon = "icon3.png", PlayerColor = "#0000FF" }
            );
            await context.SaveChangesAsync();
        }

        public PlayerRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<PlayerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestPlayerDatabase")
                .Options;
        }
    }
}
