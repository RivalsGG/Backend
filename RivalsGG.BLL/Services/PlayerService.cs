using RivalsGG.Core.Interfaces;
using RivalsGG.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.BLL.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _playerRepository.GetAllPlayersAsync();
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            return await _playerRepository.GetPlayerByIdAsync(id);
        }

        public async Task<Player> CreatePlayerAsync(Player player)
        {
            return await _playerRepository.CreatePlayerAsync(player);
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            await _playerRepository.UpdatePlayerAsync(player);
        }

        public async Task DeletePlayerAsync(int id)
        {
            await _playerRepository.DeletePlayerAsync(id);
        }
    }
}
