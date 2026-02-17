using Microsoft.AspNetCore.Mvc;
using PokemonTcgApi.DTOs;
using PokemonTcgApi.Services;

namespace PokemonTcgApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly PokemonTcgService _tcgService;

        public CardsController(PokemonTcgService tcgService)
        {
            _tcgService = tcgService;
        }

        // GET: /api/cards/search?name=charizard
        [HttpGet("search")]
        public async Task<IActionResult> SearchCards([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            var result = await _tcgService.SearchCardsByName(name);
            return Ok(result);
        }

        // GET: /api/cards/search-with-history?name=charizard
        // Main endpoint for frontend: returns cards with art, details, and price history
        [HttpGet("search-with-history")]
        public async Task<IActionResult> SearchCardsWithHistory([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            var searchResult = await _tcgService.SearchCardsByName(name);

            if (searchResult?.Data == null || !searchResult.Data.Any())
                return Ok(new { data = new List<CardWithPriceHistoryDto>() });

            var cardsWithHistory = new List<CardWithPriceHistoryDto>();

            foreach (var card in searchResult.Data)
            {
                var cardId = card.Id.ToString();
                var history = await _tcgService.GetPriceHistory(cardId);

                // Extract current price from API response
                decimal? currentPrice = null;
                string? currency = null;

                if (card.Prices?.Cardmarket?.Lowest_Near_Mint.HasValue == true)
                {
                    currentPrice = card.Prices.Cardmarket.Lowest_Near_Mint;
                    currency = card.Prices.Cardmarket.Currency ?? "EUR";
                }
                else if (card.Prices?.Tcg_Player?.Lowest_Near_Mint.HasValue == true)
                {
                    currentPrice = card.Prices.Tcg_Player.Lowest_Near_Mint;
                    currency = card.Prices.Tcg_Player.Currency ?? "USD";
                }

                // Auto-save current price to history for future graphing
                if (currentPrice.HasValue && currentPrice > 0)
                {
                    string source = currency == "EUR" ? "CardMarket" : "TCGPlayer";
                    await _tcgService.SavePriceToHistory(cardId, currentPrice.Value, source);
                }

                var cardWithHistory = new CardWithPriceHistoryDto
                {
                    Id = cardId,
                    Name = card.Name,
                    Supertype = card.Supertype,
                    Rarity = card.Rarity,
                    Hp = card.Hp,
                    Image = card.Image,
                    Artist = card.Artist?.Name,
                    EpisodeName = card.Episode?.Name,
                    EpisodeCode = card.Episode?.Code,
                    Tcgid = card.Tcgid,
                    CurrentPrice = currentPrice,
                    PriceCurrency = currency,
                    PriceHistory = history.Select(h => new PriceHistoryDto
                    {
                        RecordedAt = h.RecordedAt,
                        Price = h.Price,
                        Source = h.Source
                    }).ToList()
                };

                cardsWithHistory.Add(cardWithHistory);
            }

            return Ok(new { data = cardsWithHistory });
        }

        // POST: /api/cards/{cardId}/price-history
        // Saves current price to history - call this whenever a user views a card
        [HttpPost("{cardId}/price-history")]
        public async Task<IActionResult> SavePriceHistory(string cardId, [FromBody] SavePriceRequest request)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return BadRequest("Card ID is required.");

            if (request == null || request.Price <= 0)
                return BadRequest("Valid price is required.");

            await _tcgService.SavePriceToHistory(cardId, request.Price, request.Source ?? "Unknown");
            return Ok(new { message = "Price saved to history successfully." });
        }

        // GET: /api/cards/{cardId}/price-history
        // Returns all price history for a specific card (for graphing)
        [HttpGet("{cardId}/price-history")]
        public async Task<IActionResult> GetPriceHistory(string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return BadRequest("Card ID is required.");

            var history = await _tcgService.GetPriceHistory(cardId);

            // Convert to DTO format
            var historyDto = history.Select(h => new PriceHistoryDto
            {
                RecordedAt = h.RecordedAt,
                Price = h.Price,
                Source = h.Source
            }).ToList();

            return Ok(historyDto);
        }

        // GET: /api/cards/{cardId}/price-history/grouped
        // Returns price history grouped by source (e.g., separate lines for TCGPlayer and CardMarket)
        [HttpGet("{cardId}/price-history/grouped")]
        public async Task<IActionResult> GetPriceHistoryGrouped(string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return BadRequest("Card ID is required.");

            var historyBySource = await _tcgService.GetPriceHistoryBySource(cardId);

            // Convert to DTO format
            var result = historyBySource.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(h => new PriceHistoryDto
                {
                    RecordedAt = h.RecordedAt,
                    Price = h.Price,
                    Source = h.Source
                }).ToList()
            );

            return Ok(result);
        }
    }

    // Request model for saving price
    public class SavePriceRequest
    {
        public decimal Price { get; set; }
        public string? Source { get; set; }
    }
}