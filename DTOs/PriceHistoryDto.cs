namespace PokemonTcgApi.DTOs
{
    public class PriceHistoryDto
    {
        public DateTime RecordedAt { get; set; }
        public decimal Price { get; set; }
        public string Source { get; set; }
    }
}
