namespace TheReplacement.PokemonDamageCalc.Services
{
    using TheReplacement.PokemonDamageCalc.DataModel;
    using TheReplacement.PokemonDamageCalc.DTOs;

    public interface IDamageService
    {
        public IEnumerable<DamageRoll> GetDamageRolls(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move,
            Conditionals conditionals,
            StringPair statusConditions,
            string weather,
            string terrain);
    }
}
