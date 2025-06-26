namespace TheReplacement.PokemonDamageCalc.Client.DTOs
{
    public class MoveData
    {
        public required string Name { get; init; }
        public required int? BasePower { get; init; }
        public required string DamageClass { get; init; }
        public required string Type { get; init; }
        public required string Target { get; init; }
        public required string FlavorText { get; init; }
        public required int? Accuracy { get; init; }
    }
}
