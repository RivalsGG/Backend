using Microsoft.AspNetCore.Mvc;
using RivalsGG.BLL.Services;
using RivalsGG.Core.Models;

namespace RivalsGG.API.Controllers
{
    [Route("api/marvel/[controller]")]
    [ApiController]
    public class PlayerstatsController : ControllerBase
    {
        private readonly MarvelApiClient _marvelApiClient;

        public PlayerstatsController(MarvelApiClient marvelApiClient)
        {
            _marvelApiClient = marvelApiClient;
        }

        [HttpGet("{uid}")]
        public async Task<ActionResult<PlayerProfile>> GetPlayerProfile(long uid)
        {
            var profile = await _marvelApiClient.GetPlayerProfileAsync(uid);

            if (profile == null || profile.Uid == 0)
            {
                return NotFound($"Player with UID {uid} not found");
            }

            return Ok(profile);
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<PlayerInfo>>> GetTopPlayers([FromQuery] int limit = 10)
        {
            var topPlayers = await _marvelApiClient.GetTopPlayersAsync(limit);
            return Ok(topPlayers);
        }
    }
}
