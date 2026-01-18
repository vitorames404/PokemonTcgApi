using System;

namespace PokemonTcgApi.DTOs
{
    public class PriceGuideDto
    {
        public decimal Low { get; set; }
        public decimal Avg { get; set; }
        public decimal High { get; set; }
        public decimal Trend { get; set; }
    }
}
