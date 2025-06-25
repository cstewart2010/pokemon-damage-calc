using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public interface IStatService
    {
        public double GetAttack(bool isCritical, bool isUnaware, StattedPokemon pokemon);
        public double GetDefenseForBodyPress(bool isCritical, bool isUnaware, StattedPokemon pokemon);
        public double GetSpecialAttack(bool isCritical, bool isUnaware, StattedPokemon pokemon);
        public double GetDefense(bool isCritical, bool isUnaware, StattedPokemon pokemon);
        public double GetSpecialDefense(bool isCritical, bool isUnaware, StattedPokemon pokemon);
    }
}
