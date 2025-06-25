namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    using TheReplacement.PokemonDamageCalc.Client.DataModel;
    using TheReplacement.PokemonDamageCalc.Client.DTOs;

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
