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
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = string.Empty;

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        [JsonPropertyName("stats")]
        public PlayerStats Stats { get; set; } = new PlayerStats();

        [JsonPropertyName("heroes")]
        public Dictionary<string, HeroStats> Heroes { get; set; } = new Dictionary<string, HeroStats>();
    }
}
