namespace TheReplacement.PokemonDamageCalc.DataModel
{
    public class TypeEffectiveness
    {
        public required ICollection<string> Weaknesses { get; init; }
        public required ICollection<string> Resistances { get; init; }
        public required ICollection<string> Immunities { get; init; }
    }
}
