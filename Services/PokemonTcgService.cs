using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PokemonTcgApi.Data;
using PokemonTcgApi.DTOs;
using PokemonTcgApi.Models;

namespace PokemonTcgApi.Services
{
    public class PokemonTcgService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public PokemonTcgService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        // Method to search the cards by name.
        public async Task<PokemonCardSearchResponseDto> SearchCardsByName(string name)
        {
            try
            {
                string url = $"https://api.pokemontcg.io/v2/cards?q=name:{name}";

                // Set timeout for the request
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.GetAsync(url, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    // Log the error status code and return empty result
                    Console.WriteLine($"Pokemon TCG API error: {response.StatusCode}");
                    return new PokemonCardSearchResponseDto { Data = new List<PokemonCardDto>() };
                }

                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PokemonCardSearchResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new PokemonCardSearchResponseDto { Data = new List<PokemonCardDto>() };
            }
            catch (TaskCanceledException)
            {
                // Timeout occurred
                Console.WriteLine("Pokemon TCG API request timed out");
                return new PokemonCardSearchResponseDto { Data = new List<PokemonCardDto>() };
            }
            catch (Exception ex)
            {
                // Any other error
                Console.WriteLine($"Error calling Pokemon TCG API: {ex.Message}");
                return new PokemonCardSearchResponseDto { Data = new List<PokemonCardDto>() };
            }
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

        // Save current price to history
        public async Task SavePriceToHistory(string cardId, decimal price, string source)
        {
            var priceHistory = new PriceHistory
            {
                CardId = cardId,
                Price = price,
                RecordedAt = DateTime.UtcNow,
                Source = source
            };

            _context.PriceHistories.Add(priceHistory);
            await _context.SaveChangesAsync();
        }

        // Get price history for a specific card
        public async Task<List<PriceHistory>> GetPriceHistory(string cardId)
        {
            return await _context.PriceHistories
                .Where(ph => ph.CardId == cardId)
                .OrderBy(ph => ph.RecordedAt)
                .ToListAsync();
        }

        // Get price history grouped by source (for graphing multiple price sources)
        public async Task<Dictionary<string, List<PriceHistory>>> GetPriceHistoryBySource(string cardId)
        {
            var history = await _context.PriceHistories
                .Where(ph => ph.CardId == cardId)
                .OrderBy(ph => ph.RecordedAt)
                .ToListAsync();

            return history
                .GroupBy(ph => ph.Source)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

    }

}

