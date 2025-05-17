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
                _logger.LogInformation($"Requesting player info for UID: {uid}");

                if (string.IsNullOrWhiteSpace(uid))
                {
                    return BadRequest("A valid player UID is required");
                }

                // Use the enhanced method that tries to get more stats
                var playerStats = await _marvelApiClient.GetPlayerStatsBasicAsync(uid);

                // Check if we at least found a username
                if (string.IsNullOrWhiteSpace(playerStats.Name))
                {
                    _logger.LogWarning($"No player name found for UID: {uid}");
                    return NotFound($"No player found with UID: {uid}");
                }

                return Ok(playerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving player info for UID {uid}: {ex.Message}");
                return StatusCode(500, $"Error retrieving player info: {ex.Message}");
            }


        }
    }
}
