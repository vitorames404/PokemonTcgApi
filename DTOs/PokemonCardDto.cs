namespace PokemonTcgApi.DTOs{

public class PokemonCardDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Supertype { get; set; }
    public CardImageDto? Images { get; set; }
    public CardSetDto? Set { get; set; }
    public string? Rarity { get; set; }
    public TcgPlayerDto? Tcgplayer { get; set; }  // TCGPlayer price data from API
}
}
