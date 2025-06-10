using Microsoft.AspNetCore.Mvc;
using RivalsGG.Core.Interfaces;
using RivalsGG.Core.Models;
using RivalsGG.Core.DTOs;
using Microsoft.AspNetCore.SignalR;
using RivalsGG.API.Hubs;
using Microsoft.EntityFrameworkCore;

namespace RivalsGG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IHubContext<Playerhub> _hubContext;

        public PlayerController(IPlayerService playerService, IHubContext<Playerhub> hubContext)
        {
            _playerService = playerService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDTO>> GetPlayer(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(player);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDTO>> CreatePlayer(PlayerDTO playerDto)
        {
            var createdPlayer = await _playerService.CreatePlayerAsync(playerDto);
            await _hubContext.Clients.Group("PlayerUpdates")
            .SendAsync("PlayerCreated", createdPlayer);
            return CreatedAtAction(nameof(GetPlayer), new { id = createdPlayer.PlayerId }, createdPlayer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, PlayerDTO playerDto)
        {
            if (id != playerDto.PlayerId)
            {
                playerDto.PlayerId = id;
            }

            try
            {
                await _playerService.UpdatePlayerAsync(playerDto);
                await _hubContext.Clients.Group("PlayerUpdates")
                    .SendAsync("PlayerUpdated", playerDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            try
            {
                await _playerService.DeletePlayerAsync(id);
                await _hubContext.Clients.Group("PlayerUpdates")
                .SendAsync("PlayerDeleted", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); 
            }
        }
    }
}
