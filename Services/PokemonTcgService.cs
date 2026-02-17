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

        // Search cards by name via RapidAPI
        public async Task<PokemonCardSearchResponseDto> SearchCardsByName(string name)
        {
            try
            {
                string encodedName = Uri.EscapeDataString(name);
                string url = $"https://pokemon-tcg-api.p.rapidapi.com/cards?name={encodedName}&page=1&per_page=20";

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                var response = await _httpClient.GetAsync(url, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Pokemon TCG API error: {response.StatusCode}");
                    return new PokemonCardSearchResponseDto();
                }

                string json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<PokemonCardSearchResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new PokemonCardSearchResponseDto();
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Pokemon TCG API request timed out");
                return new PokemonCardSearchResponseDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling Pokemon TCG API: {ex.Message}");
                return new PokemonCardSearchResponseDto();
            }
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

