using System;

namespace PokemonTcgApi.DTOs
{
    public class CardMarketProductDto
    {
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public PriceGuideDto PriceGuide { get; set; }
    }
}
