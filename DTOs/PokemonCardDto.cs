using System.Text.Json.Serialization;

namespace PokemonTcgApi.DTOs
{
    public class PokemonCardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Supertype { get; set; }
        public string? Rarity { get; set; }
        [JsonConverter(typeof(FlexibleStringConverter))]
        public string? Card_Number { get; set; }  // Can be int or string in API
        public int? Hp { get; set; }
        public string? Tcgid { get; set; }
        public string? Image { get; set; }  // Direct image URL

        // Episode info (replaces Set)
        public EpisodeDto? Episode { get; set; }

        // Artist info
        public ArtistDto? Artist { get; set; }

        // RapidAPI price structure
        public RapidApiPricesDto? Prices { get; set; }

        // Catch any unmapped fields from the API (slug, type, name_numbered, links, etc.)
        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtraFields { get; set; }
    }

    // RapidAPI price structure
    public class RapidApiPricesDto
    {
        public CardMarketPriceDetailDto? Cardmarket { get; set; }
        public TcgPlayerPriceDetailDto? Tcg_Player { get; set; }
    }

    public class CardMarketPriceDetailDto
    {
        public string? Currency { get; set; }
        public decimal? Lowest_Near_Mint { get; set; }
        public decimal? Lowest_Near_Mint_DE { get; set; }
        public decimal? Lowest_Near_Mint_FR { get; set; }
        public decimal? Lowest_Near_Mint_ES { get; set; }
        public decimal? Lowest_Near_Mint_IT { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtraFields { get; set; }
    }

    public class TcgPlayerPriceDetailDto
    {
        public string? Currency { get; set; }
        public decimal? Lowest_Near_Mint { get; set; }

        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? ExtraFields { get; set; }
    }

    public class EpisodeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Code { get; set; }
        public string? Logo { get; set; }
        public int Cards_Total { get; set; }
    }

    public class ArtistDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
    }

    // Handles JSON values that can be either a number or a string
    public class FlexibleStringConverter : JsonConverter<string?>
    {
        public override string? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                System.Text.Json.JsonTokenType.Number => reader.GetInt64().ToString(),
                System.Text.Json.JsonTokenType.String => reader.GetString(),
                System.Text.Json.JsonTokenType.Null => null,
                _ => reader.GetString()
            };
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, string? value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
