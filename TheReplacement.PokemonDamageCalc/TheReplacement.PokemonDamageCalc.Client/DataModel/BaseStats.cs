namespace TheReplacement.PokemonDamageCalc.Client.DataModel
{
    public class BaseStats
    {
        public required int HP { get; set; }
        public required int Attack { get; set; }
        public required int Defense { get; set; }
        public required int SpecialAttack { get; set; }
        public required int SpecialDefense { get; set; }
        public required int Speed { get; set; }
    }
}
