using System.Text.Json;
using PokemonTcgApi.DTOs;

namespace PokemonTcgApi.Services
{        
    public class PokemonTcgService
    {
        private readonly HttpClient _httpClient;

        public PokemonTcgService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Method to search the cards by name.
        public async Task<PokemonCardSearchResponseDto> SearchCardsByName(string name)
        {
            string url = $"https://api.pokemontcg.io/v2/cards?q=name:{name}";

            // Use the HttpClient private class to make the request, using the url as parameter
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PokemonCardSearchResponseDto>(json);

            return result;
        }

    }

}

