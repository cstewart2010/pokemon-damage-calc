using TheReplacement.PokemonDamageCalc.Client.DataModel;
using TheReplacement.PokemonDamageCalc.Client.DTOs;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
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
