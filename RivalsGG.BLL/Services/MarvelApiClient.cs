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
        public async Task<IEnumerable<MarvelHero>> GetHeroesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MarvelHero>>("api/v1/heroes", _jsonOptions) ??
                   Enumerable.Empty<MarvelHero>();
        }

        public async Task<MarvelHero?> GetHeroByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<MarvelHero>($"api/v1/heroes/hero/{id}", _jsonOptions);
        }
        public async Task<RivalsPlayerStats?> GetPlayerByUidAsync(string uid)
        {
            try
            {
                _logger?.LogInformation($"Requesting player data for UID: {uid} from API");

                var endpoint = $"api/v1/player/{uid}";
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning($"API returned status code {response.StatusCode} for UID {uid}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger?.LogDebug($"Received response for UID {uid}: {content}");

                var result = JsonSerializer.Deserialize<RivalsPlayerStats>(content, _jsonOptions);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"No player data for UID {uid}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetPlayerNameByUidAsync(string uid)
        {
            try
            {
                var playerStats = await GetPlayerByUidAsync(uid);
                return playerStats?.Name ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"No player name for UID {uid}: {ex.Message}");

                try
                {
                    var endpoint = $"api/v1/player/{uid}";
                    var response = await _httpClient.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        return string.Empty;
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(content))
                    {
                        if (doc.RootElement.TryGetProperty("name", out JsonElement nameElement) &&
                            nameElement.ValueKind == JsonValueKind.String)
                        {
                            string name = nameElement.GetString();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                return name;
                            }
                        }

                        string[] possibleProperties = { "username", "player_name", "nickname", "displayName" };
                        foreach (var propName in possibleProperties)
                        {
                            if (doc.RootElement.TryGetProperty(propName, out JsonElement propElement) &&
                                propElement.ValueKind == JsonValueKind.String)
                            {
                                string propValue = propElement.GetString();
                                if (!string.IsNullOrWhiteSpace(propValue))
                                {
                                    return propValue;
                                }
                            }
                        }
                    }
                }
                catch
                {
                   
                }

                throw; 
            }
        }

    }
}



