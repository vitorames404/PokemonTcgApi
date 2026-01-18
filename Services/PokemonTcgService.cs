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
            var result = JsonSerializer.Deserialize<PokemonCardSearchResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new PokemonCardSearchResponseDto { Data = new List<PokemonCardDto>() };
        }

        public async Task<CardMarketResponseDto?> GetCardMarketPricesByName(string name)
        {
            var encodedName = Uri.EscapeDataString(name);
            var url = $"https://api.cardmarket.com/v2/products?search={encodedName}&idLanguage=1&exact=false";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // If 404 or other error, return empty result instead of throwing
                return new CardMarketResponseDto
                {
                    Product = new List<CardMarketProductDto>()
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CardMarketResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new CardMarketResponseDto { Product = new List<CardMarketProductDto>() };
        }

    }

}

