namespace PokemonTcgApi.DTOs
{
    public class CardWithPriceHistoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Supertype { get; set; }
        public string? Rarity { get; set; }
        public int? Hp { get; set; }
        public string? Image { get; set; }          // Card art URL
        public string? Artist { get; set; }          // Card artist name
        public string? EpisodeName { get; set; }     // Set/episode name (e.g. "Ascended Heroes")
        public string? EpisodeCode { get; set; }     // Set code (e.g. "ASC")
        public string? Tcgid { get; set; }           // TCG identifier
        public decimal? CurrentPrice { get; set; }   // Current market price
        public string? PriceCurrency { get; set; }   // Currency (EUR, USD)
        public List<PriceHistoryDto> PriceHistory { get; set; } = new();
    }
}
