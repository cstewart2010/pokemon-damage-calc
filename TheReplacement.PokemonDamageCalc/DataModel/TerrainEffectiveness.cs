namespace TheReplacement.PokemonDamageCalc.DataModel
{
    public class TerrainEffectiveness
    {
        public required ICollection<string> BoostedOffensiveTypes { get; init; }
        public required ICollection<string> WeakenedOffensiveTypes { get; init; }
        public required ICollection<string> AdditionalBoostedMoves { get; init; }
    }
}
