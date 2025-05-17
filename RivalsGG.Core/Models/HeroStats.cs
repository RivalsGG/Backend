using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RivalsGG.Core.Models
{
    public class HeroStats
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("matches")]
        public int Matches { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("winrate")]
        public decimal Winrate { get; set; }

        [JsonPropertyName("playtime")]
        public long Playtime { get; set; }
    }
}
