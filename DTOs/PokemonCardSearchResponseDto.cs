namespace PokemonTcgApi.DTOs
{
    public class PokemonCardSearchResponseDto
    {
        public List<PokemonCardDto> Data { get; set; } = new();
        public PagingDto? Paging { get; set; }
        public int Results { get; set; }
    }

    public class PagingDto
    {
        public int Current { get; set; }
        public int Total { get; set; }
        public int Per_Page { get; set; }
    }
}