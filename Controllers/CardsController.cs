using Microsoft.AspNetCore.Mvc;
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

        // GET: /api/cards/prices?name=charizard
        [HttpGet("prices")]
        public async Task<IActionResult> GetPrices([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            var result = await _tcgService.GetCardMarketPricesByName(name);
            return Ok(result);
        }
    }
}