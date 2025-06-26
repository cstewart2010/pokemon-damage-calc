namespace TheReplacement.PokemonDamageCalc.DTOs
{
    public class NatureData
    {
        public required string Name { get; init; }
        public required string? IncreasedStat { get; init; }
        public required string? DecreasedStat { get; init; }
    }
}
