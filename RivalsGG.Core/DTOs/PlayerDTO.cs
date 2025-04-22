using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Core.DTOs
{
    public class PlayerDTO
    {
        public int PlayerId { get; set; }

        [Required(ErrorMessage = "Player name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s_-]+$", ErrorMessage = "Player name can only use regular characters")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 20 characters")]
        public string PlayerName { get; set; } = string.Empty;

        public string? PlayerAuthId { get; set; }

        [DataType(DataType.Url)]
        [RegularExpression(@"^(https?:\/\/)?([\w\-]+(\.[\w\-]+)+\/?)([\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=]*)$", ErrorMessage = "URL can only contain basic characters")]
        public string? PlayerIcon { get; set; }

        [RegularExpression(@"^#(?:[0-9a-fA-F]{6})$", ErrorMessage = "Player color can onle be a valid hex color")]
        public string PlayerColor { get; set; } = string.Empty;
    }
}
