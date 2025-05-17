using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RivalsGG.Core.Models
{
    public class RivalsPlayerStats
    {
        [JsonPropertyName("uid")]
        public long Uid { get; set; }

        [JsonPropertyName("username")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("rank")]
        public string Rank { get; set; } = string.Empty;

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("winrate")]
        public decimal Winrate { get; set; }

        [JsonPropertyName("matches")]
        public int Matches { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonIgnore]
        public bool HasData => !string.IsNullOrEmpty(Name) && (Matches > 0 || Level > 0);
    }
}
