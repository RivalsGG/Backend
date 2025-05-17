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
            return await _httpClient.GetFromJsonAsync<IEnumerable<MarvelHero>>("heroes") ??
                   Enumerable.Empty<MarvelHero>();
        }

        public async Task<MarvelHero?> GetHeroByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<MarvelHero>($"heroes/hero/{id}");
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

    }
}

