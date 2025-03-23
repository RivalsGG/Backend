using Microsoft.EntityFrameworkCore;
using RivalsGG.Core;
using RivalsGG.Core.Models;
using RivalsGG.DAL;
using RivalsGG.DAL.Data;
using RivalsGG.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Test
{
    public class PlayerRepositoryTests
    {
        [Fact]
        public async Task GetAllPlayersAsync_ReturnsAllPlayers()
        {
            // Create a unique database for this test
            var options = new DbContextOptionsBuilder<PlayerDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestPlayerDatabase_{Guid.NewGuid()}")
                .Options;

            // Arrange
            await using var context = new PlayerDbContext(options);
            var repository = new PlayerRepository(context);

            // Seed data
            context.Players.AddRange(
                new Player { PlayerId = 1, PlayerName = "TestPlayer1", PlayerAuthId = "auth1", PlayerIcon = "icon1.png", PlayerColor = "#FF0000" },
                new Player { PlayerId = 2, PlayerName = "TestPlayer2", PlayerAuthId = "auth2", PlayerIcon = "icon2.png", PlayerColor = "#00FF00" },
                new Player { PlayerId = 3, PlayerName = "TestPlayer3", PlayerAuthId = "auth3", PlayerIcon = "icon3.png", PlayerColor = "#0000FF" }
            );
            await context.SaveChangesAsync();

            // Act
            var players = await repository.GetAllPlayersAsync();

            // Assert
            Assert.Equal(3, players.Count());  // Should be 3 players
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WithValidId_ReturnsPlayer()
        {
            // Create a unique database for this test
            var options = new DbContextOptionsBuilder<PlayerDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestPlayerDatabase_{Guid.NewGuid()}")
                .Options;

            // Arrange
            await using var context = new PlayerDbContext(options);
            var repository = new PlayerRepository(context);

            // Seed data
            context.Players.AddRange(
                new Player { PlayerId = 1, PlayerName = "TestPlayer1", PlayerAuthId = "auth1", PlayerIcon = "icon1.png", PlayerColor = "#FF0000" }
            );
            await context.SaveChangesAsync();

            // Act
            var player = await repository.GetPlayerByIdAsync(1);

            // Assert
            Assert.NotNull(player);
            Assert.Equal("TestPlayer1", player.PlayerName);
        }

        [Fact]
        public async Task CreatePlayerAsync_AddsPlayerToDatabase()
        {
            // Create a unique database for this test
            var options = new DbContextOptionsBuilder<PlayerDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestPlayerDatabase_{Guid.NewGuid()}")
                .Options;

            // Arrange
            await using var context = new PlayerDbContext(options);
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
    }
}
