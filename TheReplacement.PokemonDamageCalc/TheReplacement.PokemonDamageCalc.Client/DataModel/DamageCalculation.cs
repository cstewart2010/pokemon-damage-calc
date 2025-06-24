using PokeApiNet;
using System.Collections.ObjectModel;
using TheReplacement.PokemonDamageCalc.Client.Services;

namespace TheReplacement.PokemonDamageCalc.Client.DataModel
{
    public class DamageCalculation(
        StattedPokemon offensivePokemon,
        StattedPokemon defensivePokemon,
        Move move,
        bool isCritical,
        string offensiveStatusCondition,
        string weather,
        string terrain)
    {
        public ReadOnlyCollection<DamageRoll> Rolls { get; } = DamageHelper.GetDamageRolls(offensivePokemon, defensivePokemon, move, isCritical, offensiveStatusCondition, weather, terrain).ToList().AsReadOnly();

        public string RollRange => $"{Math.Floor(LowRoll * 10) / 10} - {Math.Floor(HighRoll * 10) / 10}%";

        private double LowRoll => (Rolls.FirstOrDefault()?.Damage ?? 0) / defensivePokemon.HP * 100;
        private double HighRoll => (Rolls.LastOrDefault()?.Damage ?? 0) / defensivePokemon.HP * 100;
    }
}
