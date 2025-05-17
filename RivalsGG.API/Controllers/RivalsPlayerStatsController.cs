using Microsoft.AspNetCore.Mvc;
using RivalsGG.BLL.Services;
using RivalsGG.Core.Models;

namespace RivalsGG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RivalsPlayerStatsController: ControllerBase
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
        public async Task<ActionResult<RivalsPlayerStats>> GetPlayerByUid(string uid)
        {
            try
            {
                _logger.LogInformation($"Requesting player stats for UID: {uid}");

                if (string.IsNullOrWhiteSpace(uid))
                {
                    return BadRequest("A valid player UID is required");
                }

                var player = await _marvelApiClient.GetPlayerByUidAsync(uid);

                if (player == null)
                {
                    return NotFound($"No player found with UID: {uid}");
                }

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving player with UID {uid}: {ex.Message}");
                return StatusCode(500, $"Error retrieving player data: {ex.Message}");
            }
        }

    }
}
