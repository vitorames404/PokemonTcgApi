namespace PokemonTcgApi.DTOs
{
    public class PokemonCardSearchResponseDto
    {
        public List<PokemonCardDto> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
    }
}