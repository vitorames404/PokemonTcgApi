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
        // Enhanced endpoint: returns cards with their price history for graphing
        [HttpGet("search-with-history")]
        public async Task<IActionResult> SearchCardsWithHistory([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            // Get cards from external API
            var searchResult = await _tcgService.SearchCardsByName(name);

            if (searchResult?.Data == null || !searchResult.Data.Any())
                return Ok(new { data = new List<CardWithPriceHistoryDto>() });

            // For each card, fetch its price history from database
            var cardsWithHistory = new List<CardWithPriceHistoryDto>();

            foreach (var card in searchResult.Data)
            {
                var history = await _tcgService.GetPriceHistory(card.Id);

                // Extract current market price from TCGPlayer data (first variant's market price)
                decimal? currentPrice = card.Tcgplayer?.Prices?.Values
                    .FirstOrDefault()?.Market;

                // Fall back to most recent price from history if TCGPlayer price not available
                if (!currentPrice.HasValue)
                {
                    currentPrice = history.OrderByDescending(h => h.RecordedAt).FirstOrDefault()?.Price;
                }

                var cardWithHistory = new CardWithPriceHistoryDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Supertype = card.Supertype,
                    Images = card.Images,
                    Set = card.Set,
                    Rarity = card.Rarity,
                    CurrentPrice = currentPrice,
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

        // GET: /api/cards/prices?name=charizard
        [HttpGet("prices")]
        public async Task<IActionResult> GetPrices([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            var result = await _tcgService.GetCardMarketPricesByName(name);
            return Ok(result);
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