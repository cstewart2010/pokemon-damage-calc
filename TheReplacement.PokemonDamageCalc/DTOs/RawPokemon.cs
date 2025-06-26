namespace TheReplacement.PokemonDamageCalc.DTOs
{
    public class RawPokemon
    {
        public required string SpeciesName { get; init; }
        public required int Weight { get; init; }
        public required ICollection<string> Types { get; init; }
        public required BaseStats Stats { get; init; }
        public required ICollection<AbilityData> Abilities { get; init; }
    }
}
