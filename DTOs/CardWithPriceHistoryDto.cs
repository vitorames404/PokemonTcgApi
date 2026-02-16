namespace PokemonTcgApi.DTOs
{
    public class CardWithPriceHistoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Supertype { get; set; }
        public CardImageDto? Images { get; set; }
        public CardSetDto? Set { get; set; }
        public string? Rarity { get; set; }
        public decimal? CurrentPrice { get; set; }
        public List<PriceHistoryDto> PriceHistory { get; set; } = new();
    }
}
