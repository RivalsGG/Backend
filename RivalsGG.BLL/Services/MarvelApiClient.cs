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
        public async Task<(bool Success, string Message)> RequestPlayerDataUpdate(string uid)
        {
            try
            {
                _logger?.LogInformation($"Requesting data update for player with UID {uid}");

                var endpoint = $"api/v1/player/{uid}/update";
                var response = await _httpClient.GetAsync(endpoint);

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(content))
                    {
                        bool success = false;
                        string message = "Unknown response";

                        if (doc.RootElement.TryGetProperty("success", out JsonElement successElement) &&
                            successElement.ValueKind == JsonValueKind.True)
                        {
                            success = true;
                        }

                        if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement) &&
                            messageElement.ValueKind == JsonValueKind.String)
                        {
                            message = messageElement.GetString() ?? string.Empty;
                        }

                        _logger?.LogInformation($"Update request for player {uid}: {(success ? "Success" : "Failed")} - {message}");
                        return (success, message);
                    }
                }
                else
                {
                    string errorMessage = "Unknown error";

                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(content))
                        {
                            if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement) &&
                                messageElement.ValueKind == JsonValueKind.String)
                            {
                                errorMessage = messageElement.GetString() ?? string.Empty;
                            }
                        }
                    }
                    catch
                    {
                        errorMessage = $"Error: {response.StatusCode}";
                    }

                    switch ((int)response.StatusCode)
                    {
                        case 400:
                            _logger?.LogWarning($"Bad request when updating player {uid}: {errorMessage}");
                            break;
                        case 401:
                            _logger?.LogWarning($"Unauthorized request when updating player {uid}: API key may be invalid");
                            break;
                        case 404:
                            _logger?.LogWarning($"Player not found when updating player {uid}");
                            break;
                        case 429:
                            _logger?.LogWarning($"Rate limit exceeded when updating player {uid}: Player may already have a pending update");
                            break;
                        case 500:
                            _logger?.LogError($"Server error when updating player {uid}: {errorMessage}");
                            break;
                        default:
                            _logger?.LogWarning($"Error updating player {uid}: Status {response.StatusCode}, {errorMessage}");
                            break;
                    }

                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Exception when requesting update for player {uid}: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }

        }
    }
}



