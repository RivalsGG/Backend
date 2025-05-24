using Microsoft.AspNetCore.Mvc;
using RivalsGG.BLL.Services;

namespace RivalsGG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerNameController : ControllerBase
    {
        private readonly MarvelApiClient _marvelApiClient;
        private readonly ILogger<PlayerNameController> _logger;

        public PlayerNameController(
            MarvelApiClient marvelApiClient,
            ILogger<PlayerNameController> logger)
        {
            _marvelApiClient = marvelApiClient;
            _logger = logger;
        }

        [HttpGet("uid/{uid}")]
        public async Task<ActionResult<string>> GetPlayerNameByUid(string uid)
        {
            try
            {
                _logger.LogInformation($"Requesting player name for UID: {uid}");

                if (string.IsNullOrWhiteSpace(uid))
                {
                    return BadRequest("A valid player UID is required");
                }
                if (!uid.All(char.IsDigit))
                {
                    return BadRequest("UID can only be number");
                }

                var playerName = await _marvelApiClient.GetPlayerNameByUidAsync(uid);

                if (string.IsNullOrWhiteSpace(playerName))
                {
                    return NotFound($"No player name found for UID: {uid}");
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
