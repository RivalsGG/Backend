using RivalsGG.Core.DTOs;
using RivalsGG.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Core.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync();
        Task<PlayerDTO> GetPlayerByIdAsync(int id);
        Task<PlayerDTO> CreatePlayerAsync(PlayerDTO playerDto);
        Task UpdatePlayerAsync(PlayerDTO playerDto);
        Task DeletePlayerAsync(int id);
    }
}
