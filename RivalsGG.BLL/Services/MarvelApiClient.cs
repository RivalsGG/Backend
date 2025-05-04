using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using RivalsGG.Core.Models;

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

            // Base URL and default headers
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
    }
}

