using Microsoft.AspNetCore.Mvc;
using RivalsGG.BLL.Services;
using RivalsGG.Core.Models;

namespace RivalsGG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RivalsPlayerStatsController : ControllerBase
    {
        private readonly MarvelApiClient _marvelApiClient;
        private readonly ILogger<RivalsPlayerStatsController> _logger;

        public RivalsPlayerStatsController(
            MarvelApiClient marvelApiClient,
            ILogger<RivalsPlayerStatsController> logger)
        {
            _marvelApiClient = marvelApiClient;
            _logger = logger;
        }

        [HttpGet("uid/{uid}")]
        public async Task<ActionResult<RivalsPlayerStats>> GetRivalsPlayerStats(string uid)
        {
            try
            {
                _logger.LogInformation($"Looking for UID: {uid}");

                if (string.IsNullOrWhiteSpace(uid))
                {
                    return BadRequest("Valid player UID missing");
                }

                var playerStats = await _marvelApiClient.GetPlayerByUidAsync(uid);

                if (playerStats == null || string.IsNullOrWhiteSpace(playerStats.Name))
                {
                    _logger.LogWarning($"No player found for UID: {uid}");
                    return NotFound($"No player found with UID: {uid}");
                }

                if (playerStats.Player != null)
                {
                    if (int.TryParse(playerStats.Player.Level, out int level))
                    {
                        playerStats.Level = level;
                    }

                    if (playerStats.Player.Rank != null)
                    {
                        playerStats.Rank = playerStats.Player.Rank.Rank;
                    }
                }

                if (playerStats.OverallStats != null)
                {
                    playerStats.Matches = playerStats.OverallStats.TotalMatches;
                    playerStats.Wins = playerStats.OverallStats.TotalWins;
                    playerStats.Losses = playerStats.Matches - playerStats.Wins;

                    if (playerStats.Matches > 0)
                    {
                        playerStats.Winrate = (decimal)playerStats.Wins / playerStats.Matches;
                    }
                }

                return Ok(playerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving player stats for UID {uid}: {ex.Message}");
                return StatusCode(500, $"Error retrieving player stats: {ex.Message}");
            }
        }

        [HttpGet("summary/{uid}")]
        public async Task<ActionResult<object>> GetPlayerSummary(string uid)
        {
            try
            {
                _logger.LogInformation($"Requesting player summary for UID: {uid}");

                if (string.IsNullOrWhiteSpace(uid))
                {
                    return BadRequest("A valid player UID is required");
                }

                // Get full player stats
                var playerStats = await _marvelApiClient.GetPlayerByUidAsync(uid);

                if (playerStats == null || string.IsNullOrWhiteSpace(playerStats.Name))
                {
                    _logger.LogWarning($"No player found for UID: {uid}");
                    return NotFound($"No player found with UID: {uid}");
                }

                int level = 0;
                if (playerStats.Player?.Level != null)
                {
                    int.TryParse(playerStats.Player.Level, out level);
                }

                string rank = playerStats.Player?.Rank?.Rank ?? string.Empty;

                int totalMatches = playerStats.OverallStats?.TotalMatches ?? 0;
                int totalWins = playerStats.OverallStats?.TotalWins ?? 0;
                int totalLosses = totalMatches - totalWins;
                decimal winrate = totalMatches > 0 ? (decimal)totalWins / totalMatches : 0;

                var summary = new
                {
                    uid = playerStats.Uid,
                    username = playerStats.Name,
                    level = level,
                    rank = rank,
                    totalMatches = totalMatches,
                    totalWins = totalWins,
                    totalLosses = totalLosses,
                    winrate = Math.Round(winrate * 100, 2),
                    avatar = playerStats.Player?.Icon?.IconUrl
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving player summary for UID {uid}: {ex.Message}");
                return StatusCode(500, $"Error retrieving player summary: {ex.Message}");
            }
        }

        [HttpGet("name/{uid}")]
        public async Task<ActionResult<object>> GetPlayerName(string uid)
        {
            try
            {
                _logger.LogInformation($"Looking for player name belonging to UID: {uid}");

                if (string.IsNullOrWhiteSpace(uid))
                {
                    return BadRequest("A valid UID is required");
                }

                var playerName = await _marvelApiClient.GetPlayerNameByUidAsync(uid);

                if (string.IsNullOrWhiteSpace(playerName))
                {
                    return NotFound($"No player found with UID: {uid}");
                }

                return Ok(new { uid = uid, username = playerName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving player name for UID {uid}: {ex.Message}");
                return StatusCode(500, $"Error retrieving player name: {ex.Message}");
            }
        }

    }
}
