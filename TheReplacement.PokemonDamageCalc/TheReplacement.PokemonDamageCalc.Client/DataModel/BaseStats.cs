namespace TheReplacement.PokemonDamageCalc.Client.DataModel
{
    public class BaseStats
    {
        public required int HP { get; init; }
        public required int Attack { get; init; }
        public required int Defense { get; init; }
        public required int SpecialAttack { get; init; }
        public required int SpecialDefense { get; init; }
        public required int Speed { get; init; }
    }
}
