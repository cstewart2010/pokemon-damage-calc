namespace TheReplacement.PokemonDamageCalc.Client.Domain
{
    using TheReplacement.PokemonDamageCalc.Client.DataModel;
    using TheReplacement.PokemonDamageCalc.Client.Services;

    public class StatService : IStatService
    {
        public double GetAttack(
            bool isCritical,
            bool isUnaware,
            StattedPokemon pokemon)
        {
            var attack = pokemon.Attack;
            if ((isCritical && pokemon.Stages.Attack < 0) || isUnaware)
            {
                var temp = pokemon.Stages.Attack;
                pokemon.Stages.Attack = 0;
                attack = pokemon.Attack;
                pokemon.Stages.Attack = temp;
            }

            return attack;
        }

        public double GetDefenseForBodyPress(
            bool isCritical,
            bool isUnaware,
            StattedPokemon pokemon)
        {
            var attack = pokemon.Defense;
            if ((isCritical && pokemon.Stages.Defense < 0) || isUnaware)
            {
                var temp = pokemon.Stages.Defense;
                pokemon.Stages.Defense = 0;
                attack = pokemon.Defense;
                pokemon.Stages.Defense = temp;
            }

            return attack;
        }

        public double GetSpecialAttack(
            bool isCritical,
            bool isUnaware,
            StattedPokemon pokemon)
        {
            var attack = pokemon.SpecialAttack;
            if ((isCritical && pokemon.Stages.SpecialAttack < 0) || isUnaware)
            {
                var temp = pokemon.Stages.SpecialAttack;
                pokemon.Stages.SpecialAttack = 0;
                attack = pokemon.SpecialAttack;
                pokemon.Stages.SpecialAttack = temp;
            }

            return attack;
        }

        public double GetDefense(
            bool isCritical,
            bool isUnaware,
            StattedPokemon pokemon)
        {
            var defense = pokemon.Defense;
            if ((isCritical && pokemon.Stages.Defense > 0) || isUnaware)
            {
                var temp = pokemon.Stages.Defense;
                pokemon.Stages.Defense = 0;
                defense = pokemon.Defense;
                pokemon.Stages.Defense = temp;
            }

            return defense;
        }

        public double GetSpecialDefense(
            bool isCritical,
            bool isUnaware,
            StattedPokemon pokemon)
        {
            var defense = pokemon.SpecialDefense;
            if ((isCritical && pokemon.Stages.SpecialDefense > 0) || isUnaware)
            {
                var temp = pokemon.Stages.SpecialDefense;
                pokemon.Stages.SpecialDefense = 0;
                defense = pokemon.SpecialDefense;
                pokemon.Stages.SpecialDefense = temp;
            }

            return defense;
        }
    }
}
