using PokeApiNet;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public interface IDamageService
    {
        public IEnumerable<DamageRoll> GetDamageRolls(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            Conditionals conditionals,
            StringPair statusConditions,
            string weather,
            string terrain);
    }
}
