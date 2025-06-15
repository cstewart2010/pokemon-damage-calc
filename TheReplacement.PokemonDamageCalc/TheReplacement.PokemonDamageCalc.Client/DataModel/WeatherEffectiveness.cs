namespace TheReplacement.PokemonDamageCalc.Client.DataModel
{
    public class WeatherEffectiveness
    {
        public required ICollection<string> BoostedOffensiveTypes { get; init; }
        public required ICollection<string> BoostedDefensiveTypes { get; init; }
        public required ICollection<string> WeakenedOffensiveTypes { get; init; }
        public required ICollection<string> AdditionalBoostedMoves { get; init; }
    }
}
