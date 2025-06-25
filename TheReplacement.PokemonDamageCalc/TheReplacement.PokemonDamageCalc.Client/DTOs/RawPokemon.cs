namespace TheReplacement.PokemonDamageCalc.Client.DTOs
{
    using TheReplacement.PokemonDamageCalc.Client.DataModel;

    public class RawPokemon
    {
        public required string SpeciesName { get; init; }
        public required int Weight { get; init; }
        public required ICollection<string> Types { get; init; }
        public required BaseStats Stats { get; init; }
        public required ICollection<AbilityData> Abilities { get; init; }
    }
}
