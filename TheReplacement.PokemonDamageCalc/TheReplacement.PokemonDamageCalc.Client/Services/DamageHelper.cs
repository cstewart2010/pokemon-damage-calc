using PokeApiNet;
using System.Net.NetworkInformation;
using TheReplacement.PokemonDamageCalc.Client.Constants;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public static class DamageHelper
    {
        public static double GetTypeEffectiveness(string moveTypeName, StattedPokemon defendingPokemon, bool ignoreResistances = false, bool ignoreImmunities = false)
        {
            var effectiveness = 1;

            var pokemonTypes = defendingPokemon.Types.Select(x => Types.EffectivenessChart[x]).ToList();
            foreach (var type in pokemonTypes)
            {
                if (type.Weaknesses.Contains(moveTypeName))
                {
                    effectiveness *= 2;
                }
                else if (type.Resistances.Contains(moveTypeName) && !ignoreResistances)
                {
                    effectiveness /= 2;
                }
                if (type.Immunities.Contains(moveTypeName) && !ignoreImmunities)
                {
                    return 0;
                }
            }

            return effectiveness;
        }

        public static double GetStab(string moveTypeName, StattedPokemon offendingPokemon, string abilityName)
        {
            if (offendingPokemon.Types.Contains(moveTypeName))
            {
                if (abilityName == AbilityNames.Adaptability)
                {
                    return 2;
                }

                return 1.5;
            }

            return 1;
        }

        public static double GetStatusMultiplier(StattedPokemon offendingPokemon, Move move, string status)
        {
            double statusMultiplier = 1;
            if (offendingPokemon.Ability == AbilityNames.Guts && status != Statuses.Healthy)
            {
                statusMultiplier = move.Name == MoveNames.Facade
                    ? 3
                    : 1.5;
            }
            else if (move.Name == MoveNames.Facade && status != Statuses.Healthy)
            {
                statusMultiplier = 2;
            }
            else if (offendingPokemon.Ability == AbilityNames.ToxicBoost && status == Statuses.Poisoned)
            {
                statusMultiplier = move.Name == MoveNames.Facade
                    ? 3
                    : 1.5;
            }
            else if (status == Statuses.Burn)
            {
                statusMultiplier = 0.5;
            }

            return statusMultiplier;
        }

        public static double GetWeatherMultiplier(string weather, Move move, StattedPokemon defendingPokemon)
        {
            double multiplier = 1;
            var effectiveness = Weather.EffectivenessChart[weather];
            var isBoostMove = effectiveness.AdditionalBoostedMoves.Contains(move.Type.Name);
            if (effectiveness.WeakenedOffensiveTypes.Contains(move.Type.Name) && !isBoostMove)
            {
                multiplier = 0.5;
            }
            else if (effectiveness.BoostedOffensiveTypes.Contains(move.Type.Name))
            {
                multiplier = 1.5;
            }
            if (isBoostMove)
            {
                multiplier *= 1.5;
            }
            var isBoostDefense = defendingPokemon.Types.Any(effectiveness.BoostedDefensiveTypes.Contains);
            if (isBoostDefense)
            {
                multiplier *= 0.5;
            }

            return multiplier;
        }

        public static double GetTerrainMultiplier(string terrain, Move move, StattedPokemon offendingPokemon, StattedPokemon defendingPokemon)
        {
            double multiplier = 1;
            if (offendingPokemon.Ability == AbilityNames.Levitate || offendingPokemon.Types.Any(type => type == Types.Flying))
            {
                if (!(offendingPokemon.Ability == AbilityNames.QuarkDrive && terrain == Terrain.ElectricTerrain))
                {
                    return multiplier;
                }
            }
            var effectiveness = Terrain.EffectivenessChart[terrain];
            var isBoostMove = effectiveness.AdditionalBoostedMoves.Contains(move.Type.Name);
            if (effectiveness.WeakenedOffensiveTypes.Contains(move.Type.Name) && !isBoostMove)
            {
                multiplier = 0.5;
            }
            else if (effectiveness.BoostedOffensiveTypes.Contains(move.Type.Name))
            {
                multiplier = 1.5;
            }
            if (isBoostMove)
            {
                multiplier *= 1.5;
            }

            return multiplier;
        }
    }
}
