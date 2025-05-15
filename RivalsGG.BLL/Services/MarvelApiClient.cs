using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using RivalsGG.Core.Models;
using System.Text.Json;

namespace RivalsGG.BLL.Services
{
    public class MarvelApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MarvelApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["MarvelRivalsApi:ApiKey"] ??
                     throw new InvalidOperationException("Marvel Rivals API key is not configured");

            _httpClient.BaseAddress = new Uri("https://marvelrivalsapi.com/api/v1/");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
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
        public async Task<PlayerProfile> GetPlayerProfileAsync(long uid)
        {
            try
            {
                var response = await _httpClient.GetAsync($"player/{uid}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<PlayerProfile>(content) ??
                       new PlayerProfile { Uid = uid };
            }
            catch (HttpRequestException)
            {
                return new PlayerProfile { Uid = uid };
            }
        }

        public async Task<IEnumerable<PlayerProfile>> GetTopPlayersAsync(int limit = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"players/top?limit={limit}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<IEnumerable<PlayerProfile>>(content) ??
                       Enumerable.Empty<PlayerProfile>();
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<PlayerProfile>();
            }
        }
    }
}

