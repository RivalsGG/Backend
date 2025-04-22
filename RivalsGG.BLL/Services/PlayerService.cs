using RivalsGG.Core.Interfaces;
using RivalsGG.Core.Models;
using RivalsGG.Core.DTOs;
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

        public async Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllPlayersAsync();
            return players.Select(MapToDto);
        }

        public async Task<PlayerDTO> GetPlayerByIdAsync(int id)
        {
            var player = await _playerRepository.GetPlayerByIdAsync(id);
            return player != null ? MapToDto(player) : null;
        }

        public async Task<PlayerDTO> CreatePlayerAsync(PlayerDTO playerDto)
        {
            var player = MapToEntity(playerDto);
            var createdPlayer = await _playerRepository.CreatePlayerAsync(player);
            return MapToDto(createdPlayer);
        }

        public async Task UpdatePlayerAsync(PlayerDTO playerDto)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(playerDto.PlayerId);

            if (existingPlayer == null)
                throw new KeyNotFoundException($"Player with id {playerDto.PlayerId} not found");

            existingPlayer.PlayerName = playerDto.PlayerName;
            existingPlayer.PlayerIcon = playerDto.PlayerIcon ?? string.Empty;
            existingPlayer.PlayerColor = playerDto.PlayerColor;

            await _playerRepository.UpdatePlayerAsync(existingPlayer);
        }

        public async Task UpdatePlayerAuthAsync(int playerId, string authId)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(playerId);

            if (existingPlayer == null)
                throw new KeyNotFoundException($"Player with id {playerId} not found");

            existingPlayer.PlayerAuthId = authId;
            await _playerRepository.UpdatePlayerAsync(existingPlayer);
        }

        public async Task DeletePlayerAsync(int id)
        {
            await _playerRepository.DeletePlayerAsync(id);
        }

        private PlayerDTO MapToDto(Player player)
        {
            return new PlayerDTO
            {
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                PlayerAuthId = player.PlayerAuthId,
                PlayerIcon = player.PlayerIcon,
                PlayerColor = player.PlayerColor
            };
        }

        private Player MapToEntity(PlayerDTO dto)
        {
            return new Player
            {
                PlayerId = dto.PlayerId,
                PlayerName = dto.PlayerName,
                PlayerAuthId = dto.PlayerAuthId ?? string.Empty,
                PlayerIcon = dto.PlayerIcon ?? string.Empty,
                PlayerColor = dto.PlayerColor
            };
        }
    }
}
