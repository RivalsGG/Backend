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

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("player")]
        public PlayerInfo? Player { get; set; }

        [JsonPropertyName("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonPropertyName("overall_stats")]
        public OverallStats? OverallStats { get; set; }
        [JsonPropertyName("updates")]
        public UpdatesInfo? Updates { get; set; }

        public string Rank { get; set; } = string.Empty;
        public int Level { get; set; }
        public decimal Winrate { get; set; }
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        [JsonIgnore]
        public string Username => Name; 
    }

    public class PlayerInfo
    {
        [JsonPropertyName("uid")]
        public long Uid { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("rank")]
        public RankInfo? Rank { get; set; }

        [JsonPropertyName("icon")]
        public IconInfo? Icon { get; set; }
    }

    public class RankInfo
    {
        [JsonPropertyName("rank")]
        public string Rank { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }
    }

    public class IconInfo
    {
        [JsonPropertyName("player_icon_id")]
        public string PlayerId { get; set; } = string.Empty;

        [JsonPropertyName("player_icon")]
        public string IconUrl { get; set; } = string.Empty;
    }

    public class OverallStats
    {
        [JsonPropertyName("total_matches")]
        public int TotalMatches { get; set; }

        [JsonPropertyName("total_wins")]
        public int TotalWins { get; set; }
    }
    public class UpdatesInfo
    {
        [JsonPropertyName("info_update_time")]
        public string InfoUpdateTime { get; set; } = string.Empty;

        [JsonPropertyName("last_history_update")]
        public string? LastHistoryUpdate { get; set; }

        [JsonPropertyName("last_inserted_match")]
        public string? LastInsertedMatch { get; set; }

        [JsonPropertyName("last_update_request")]
        public string? LastUpdateRequest { get; set; }
    }
}