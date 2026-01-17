namespace PokemonTcgApi.Models
{   
    public class PriceHistory
    {
        public int Id { get; set; }  // Auto-increment primary key
        public string CardId { get; set; }  // Foreign key
        public decimal Price { get; set; }
        public DateTime RecordedAt { get; set; }
        public string Source { get; set; }  // "TCGPlayer" or "CardMarket"       
   }
}