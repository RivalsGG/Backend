using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using RivalsGG.Core.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RivalsGG.BLL.Services
{
    public class MarvelApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<MarvelApiClient> _logger;

        public MarvelApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["MarvelRivalsApi:ApiKey"] ??
                     throw new InvalidOperationException("Marvel Rivals API key is not configured");

            _httpClient.BaseAddress = new Uri("https://marvelrivalsapi.com/");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
        }



        public async Task<RivalsPlayerStats> GetPlayerByUidAsync(string uid)
        {
            try
            {
                var endpoint = $"api/v1/player/{uid}";
                _logger?.LogInformation($"Requesting player with UID {uid} from endpoint: {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger?.LogDebug($"Response content: {content}");

                var result = JsonSerializer.Deserialize<RivalsPlayerStats>(content, _jsonOptions);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting player with UID {uid}: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<MarvelHero>> GetHeroesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MarvelHero>>("api/v1/heroes") ??
                   Enumerable.Empty<MarvelHero>();
        }

        public async Task<MarvelHero?> GetHeroByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<MarvelHero>($"api/v1/heroes/hero/{id}");
        }
        public async Task<string> GetPlayerNameByUidAsync(string uid)
        {
            try
            {
                var endpoint = $"api/v1/player/{uid}";
                _logger?.LogInformation($"Requesting player name for UID {uid} from endpoint: {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning($"API returned status code {response.StatusCode} for UID {uid}");
                    return string.Empty;
                }

                var content = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    if (doc.RootElement.TryGetProperty("username", out JsonElement usernameElement) &&
                        usernameElement.ValueKind == JsonValueKind.String)
                    {
                        string username = usernameElement.GetString();

                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            _logger?.LogInformation($"Successfully retrieved username '{username}' for UID {uid}");
                            return username;
                        }
                    }

                    string[] possibleProperties = { "name", "player_name", "nickname", "displayName" };

                    foreach (var propName in possibleProperties)
                    {
                        if (doc.RootElement.TryGetProperty(propName, out JsonElement nameElement) &&
                            nameElement.ValueKind == JsonValueKind.String)
                        {
                            string name = nameElement.GetString();

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                _logger?.LogInformation($"Successfully retrieved {propName} '{name}' for UID {uid}");
                                return name;
                            }
                        }
                    }
                }

                _logger?.LogWarning($"No username found in API response for UID {uid}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting player name for UID {uid}: {ex.Message}");
                throw;
            }
        }


        public async Task<RivalsPlayerStats> GetRivalsPlayerstatsAsync(string uid)
        {
            try
            {
                var endpoint = $"api/v1/player/{uid}";
                _logger?.LogInformation($"Requesting essential player info for UID {uid} from endpoint: {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning($"API returned status code {response.StatusCode} for UID {uid}");
                    return new RivalsPlayerStats { Uid = long.Parse(uid) };
                }

                var content = await response.Content.ReadAsStringAsync();

                // Use JsonDocument for more flexible parsing
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    var playerInfo = new RivalsPlayerStats { Uid = long.Parse(uid) };

                    // Extract username (already know this works)
                    if (doc.RootElement.TryGetProperty("username", out JsonElement usernameElement) &&
                        usernameElement.ValueKind == JsonValueKind.String)
                    {
                        playerInfo.Name = usernameElement.GetString() ?? string.Empty;
                    }

                    // Try to extract level
                    if (doc.RootElement.TryGetProperty("level", out JsonElement levelElement))
                    {
                        if (levelElement.ValueKind == JsonValueKind.Number)
                        {
                            playerInfo.Level = levelElement.GetInt32();
                        }
                        else if (levelElement.ValueKind == JsonValueKind.String &&
                                int.TryParse(levelElement.GetString(), out int levelValue))
                        {
                            playerInfo.Level = levelValue;
                        }
                    }

                    // Try to extract rank
                    string[] possibleRankProperties = { "rank", "tier", "league" };
                    foreach (var propName in possibleRankProperties)
                    {
                        if (doc.RootElement.TryGetProperty(propName, out JsonElement rankElement) &&
                            rankElement.ValueKind == JsonValueKind.String)
                        {
                            playerInfo.Rank = rankElement.GetString() ?? string.Empty;
                            break;
                        }
                    }

                    // Try to extract stats
                    if (doc.RootElement.TryGetProperty("stats", out JsonElement statsElement))
                    {
                        // Matches
                        if (statsElement.TryGetProperty("matches", out JsonElement matchesElement) &&
                            matchesElement.ValueKind == JsonValueKind.Number)
                        {
                            playerInfo.Matches = matchesElement.GetInt32();
                        }

                        // Wins
                        if (statsElement.TryGetProperty("wins", out JsonElement winsElement) &&
                            winsElement.ValueKind == JsonValueKind.Number)
                        {
                            playerInfo.Wins = winsElement.GetInt32();
                        }

                        // Losses
                        if (statsElement.TryGetProperty("losses", out JsonElement lossesElement) &&
                            lossesElement.ValueKind == JsonValueKind.Number)
                        {
                            playerInfo.Losses = lossesElement.GetInt32();
                        }

                        // Winrate
                        if (statsElement.TryGetProperty("winrate", out JsonElement winrateElement))
                        {
                            if (winrateElement.ValueKind == JsonValueKind.Number)
                            {
                                playerInfo.Winrate = winrateElement.GetDecimal();
                            }
                            else if (winrateElement.ValueKind == JsonValueKind.String &&
                                     decimal.TryParse(winrateElement.GetString(), out decimal winrateValue))
                            {
                                playerInfo.Winrate = winrateValue;
                            }
                        }
                        else if (playerInfo.Matches > 0)
                        {
                            // Calculate winrate if not provided but we have matches
                            playerInfo.Winrate = playerInfo.Matches > 0
                                ? (decimal)playerInfo.Wins / playerInfo.Matches
                                : 0;
                        }
                    }
                    else
                    {
                        // If no stats object, try to find stats at root level
                        string[] statProperties = { "matches", "wins", "losses", "winrate" };

                        foreach (var prop in statProperties)
                        {
                            if (doc.RootElement.TryGetProperty(prop, out JsonElement statElement) &&
                                statElement.ValueKind == JsonValueKind.Number)
                            {
                                switch (prop)
                                {
                                    case "matches":
                                        playerInfo.Matches = statElement.GetInt32();
                                        break;
                                    case "wins":
                                        playerInfo.Wins = statElement.GetInt32();
                                        break;
                                    case "losses":
                                        playerInfo.Losses = statElement.GetInt32();
                                        break;
                                    case "winrate":
                                        playerInfo.Winrate = statElement.GetDecimal();
                                        break;
                                }
                            }
                        }
                    }

                    return playerInfo;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting essential player info for UID {uid}: {ex.Message}");
                throw;
            }
        }
        public async Task<RivalsPlayerStats> GetPlayerStatsBasicAsync(string uid)
        {
            try
            {
                var endpoint = $"api/v1/player/{uid}";
                _logger?.LogInformation($"Requesting basic player stats for UID {uid} from endpoint: {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);

                // Initialize the stats object with the UID
                var playerStats = new RivalsPlayerStats
                {
                    Uid = long.Parse(uid)
                };

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning($"API returned status code {response.StatusCode} for UID {uid}");
                    return playerStats; // Return empty stats with just the UID
                }

                var content = await response.Content.ReadAsStringAsync();

                // Use lightweight JsonDocument instead of full deserialization
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    // Extract username
                    if (doc.RootElement.TryGetProperty("username", out JsonElement usernameElement) &&
                        usernameElement.ValueKind == JsonValueKind.String)
                    {
                        playerStats.Name = usernameElement.GetString() ?? string.Empty;
                    }

                    // Try to extract level
                    if (doc.RootElement.TryGetProperty("level", out JsonElement levelElement))
                    {
                        if (levelElement.ValueKind == JsonValueKind.Number)
                        {
                            playerStats.Level = levelElement.GetInt32();
                        }
                        else if (levelElement.ValueKind == JsonValueKind.String &&
                                 int.TryParse(levelElement.GetString(), out int level))
                        {
                            playerStats.Level = level;
                        }
                    }

                    // Try common rank property names
                    string[] rankProperties = { "rank", "tier", "league", "division" };
                    foreach (var prop in rankProperties)
                    {
                        if (doc.RootElement.TryGetProperty(prop, out JsonElement rankElement) &&
                            rankElement.ValueKind == JsonValueKind.String)
                        {
                            string rank = rankElement.GetString() ?? string.Empty;
                            if (!string.IsNullOrWhiteSpace(rank))
                            {
                                playerStats.Rank = rank;
                                break;
                            }
                        }
                    }

                    // Try to extract stats from a nested 'stats' object if it exists
                    if (doc.RootElement.TryGetProperty("stats", out JsonElement statsElement))
                    {
                        ExtractStats(statsElement, playerStats);
                    }
                    else
                    {
                        // If no stats object, try finding stats at the root level
                        ExtractStats(doc.RootElement, playerStats);
                    }

                    // If we have wins and matches but no winrate, calculate it
                    if (playerStats.Winrate == 0 && playerStats.Matches > 0 && playerStats.Wins > 0)
                    {
                        playerStats.Winrate = (decimal)playerStats.Wins / playerStats.Matches;
                    }

                    // If we have matches but no wins/losses, try to infer them
                    if (playerStats.Matches > 0 && playerStats.Wins == 0 && playerStats.Losses == 0 && playerStats.Winrate > 0)
                    {
                        playerStats.Wins = (int)(playerStats.Matches * playerStats.Winrate);
                        playerStats.Losses = playerStats.Matches - playerStats.Wins;
                    }
                }

                // If username is still empty, try to get it specifically
                if (string.IsNullOrWhiteSpace(playerStats.Name))
                {
                    var name = await GetPlayerNameByUidAsync(uid);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        playerStats.Name = name;
                    }
                }

                return playerStats;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting basic player stats for UID {uid}: {ex.Message}");

                // On error, try to at least get the player name
                try
                {
                    var name = await GetPlayerNameByUidAsync(uid);
                    return new RivalsPlayerStats
                    {
                        Uid = long.Parse(uid),
                        Name = name ?? string.Empty
                    };
                }
                catch
                {
                    // If everything fails, just return the UID
                    return new RivalsPlayerStats { Uid = long.Parse(uid) };
                }
            }
        }

        // Helper method to extract stats from a JsonElement
        private void ExtractStats(JsonElement element, RivalsPlayerStats stats)
        {
            // Try to extract matches
            if (element.TryGetProperty("matches", out JsonElement matchesElement))
            {
                if (matchesElement.ValueKind == JsonValueKind.Number)
                {
                    stats.Matches = matchesElement.GetInt32();
                }
                else if (matchesElement.ValueKind == JsonValueKind.String &&
                         int.TryParse(matchesElement.GetString(), out int matches))
                {
                    stats.Matches = matches;
                }
            }

            // Try to extract wins
            if (element.TryGetProperty("wins", out JsonElement winsElement))
            {
                if (winsElement.ValueKind == JsonValueKind.Number)
                {
                    stats.Wins = winsElement.GetInt32();
                }
                else if (winsElement.ValueKind == JsonValueKind.String &&
                         int.TryParse(winsElement.GetString(), out int wins))
                {
                    stats.Wins = wins;
                }
            }

            // Try to extract losses
            if (element.TryGetProperty("losses", out JsonElement lossesElement))
            {
                if (lossesElement.ValueKind == JsonValueKind.Number)
                {
                    stats.Losses = lossesElement.GetInt32();
                }
                else if (lossesElement.ValueKind == JsonValueKind.String &&
                         int.TryParse(lossesElement.GetString(), out int losses))
                {
                    stats.Losses = losses;
                }
            }

            // Try to extract winrate
            if (element.TryGetProperty("winrate", out JsonElement winrateElement))
            {
                if (winrateElement.ValueKind == JsonValueKind.Number)
                {
                    stats.Winrate = winrateElement.GetDecimal();
                }
                else if (winrateElement.ValueKind == JsonValueKind.String &&
                         decimal.TryParse(winrateElement.GetString(), out decimal winrate))
                {
                    stats.Winrate = winrate;
                }
            }
            // Also check alternate property names
            else if (element.TryGetProperty("win_rate", out winrateElement) ||
                     element.TryGetProperty("win_ratio", out winrateElement))
            {
                if (winrateElement.ValueKind == JsonValueKind.Number)
                {
                    stats.Winrate = winrateElement.GetDecimal();
                }
                else if (winrateElement.ValueKind == JsonValueKind.String &&
                         decimal.TryParse(winrateElement.GetString(), out decimal winrate))
                {
                    stats.Winrate = winrate;
                }
            }
        }
    }
}



