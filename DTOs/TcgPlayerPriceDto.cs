namespace PokemonTcgApi.DTOs
{
    // TCGPlayer price data included in Pokemon TCG API response
    public class TcgPlayerDto
    {
        public Dictionary<string, PriceVariantDto>? Prices { get; set; }
    }

    public class PriceVariantDto
    {
        public decimal? Low { get; set; }
        public decimal? Mid { get; set; }
        public decimal? High { get; set; }
        public decimal? Market { get; set; }  // Current market price
        public decimal? DirectLow { get; set; }
    }
}
