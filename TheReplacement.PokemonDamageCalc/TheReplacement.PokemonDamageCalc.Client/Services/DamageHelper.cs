using PokeApiNet;
using TheReplacement.PokemonDamageCalc.Client.Constants;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public static class DamageHelper
    {
        private static readonly ICollection<string> SpecialMovesThatDoPhysicalDamage = [MoveNames.Psyshock, MoveNames.Psystrike, MoveNames.SecretSword];

        public static IEnumerable<DamageRoll> GetDamageRolls(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            bool isCritical,
            string offensiveStatusCondition,
            string weather,
            string terrain)
        {
            double critMultiplier = isCritical
                ? offensivePokemon.Ability == AbilityNames.Adaptability
                    ? 2
                    : 1.5
                : 1;
            double statusMultiplier = 1;
            int basePower = move.Power ?? 0;
            if (move.DamageClass.Name == DamageClasses.Physical)
            {
                statusMultiplier = GetStatusMultiplier(offensivePokemon, move, offensiveStatusCondition);
            }
            var (attack, defense) = GetStats(offensivePokemon, defensivePokemon, move, isCritical);
            if (attack == 0)
            {
                return [];
            }
            var typeEffectivenessMultiplier = GetTypeEffectivenessMultiplier(move.Type.Name, defensivePokemon);
            var stab = GetStab(move.Type.Name, offensivePokemon, offensivePokemon.Ability);
            var weatherMultiplier = GetWeatherMultiplier(weather, move, defensivePokemon);
            var terrainMultiplier = GetTerrainMultiplier(terrain, move, offensivePokemon, defensivePokemon);
            var weatherDefenseMultiplier = GetWeatherDefenseMultiplier(weather, move, defensivePokemon);
            defense *= weatherDefenseMultiplier;
            var rawDamage = Math.Floor((2 * offensivePokemon.Level / 5.0 + 2) * basePower * (attack / defense) / 50 + 2);
            rawDamage = Math.Floor(rawDamage * critMultiplier);
            rawDamage = Math.Floor(rawDamage * statusMultiplier);
            rawDamage = Math.Floor(rawDamage * typeEffectivenessMultiplier);
            rawDamage = Math.Floor(rawDamage * stab);
            rawDamage = Math.Floor(rawDamage * weatherMultiplier);
            rawDamage = Math.Floor(rawDamage * terrainMultiplier);
            return Enumerable.Range(85, 16).Select(roll => new DamageRoll
            {
                Roll = roll,
                Damage = Math.Floor(rawDamage * (roll / 100.0))
            });
        }

        private static (double EffectiveAttackStat, double EffectiveDefenseStat) GetStats(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            bool isCritical)
        {
            double attack, defense;
            if (move.DamageClass.Name == DamageClasses.Physical)
            {
                attack = move.Name == MoveNames.BodyPress
                    ? StatHelper.GetDefenseForBodyPress(isCritical, defensivePokemon.Ability == AbilityNames.Unaware, offensivePokemon)
                    : move.Name == MoveNames.FoulPlay
                        ? StatHelper.GetAttack(isCritical, defensivePokemon.Ability == AbilityNames.Unaware, defensivePokemon)
                        : StatHelper.GetAttack(isCritical, defensivePokemon.Ability == AbilityNames.Unaware, offensivePokemon);
                defense = StatHelper.GetDefense(isCritical, offensivePokemon.Ability == AbilityNames.Unaware || move.Name == MoveNames.SacredSword, defensivePokemon);
                return (attack, defense);
            }
            else if (move.DamageClass.Name == DamageClasses.Special)
            {
                attack = StatHelper.GetSpecialAttack(isCritical, defensivePokemon.Ability == AbilityNames.Unaware, offensivePokemon);
                defense = SpecialMovesThatDoPhysicalDamage.Contains(move.Name)
                    ? StatHelper.GetDefense(isCritical, offensivePokemon.Ability == AbilityNames.Unaware, defensivePokemon)
                    : StatHelper.GetSpecialDefense(isCritical, offensivePokemon.Ability == AbilityNames.Unaware, defensivePokemon);
                return (attack, defense);
            }

            return (0, 0);
        }

        private static double GetTypeEffectivenessMultiplier(string moveTypeName, StattedPokemon defendingPokemon, bool ignoreResistances = false, bool ignoreImmunities = false)
        {
            double effectiveness = 1;

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

        private static double GetStab(string moveTypeName, StattedPokemon offendingPokemon, string abilityName)
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

        private static double GetStatusMultiplier(StattedPokemon offendingPokemon, Move move, string status)
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

        private static double GetWeatherMultiplier(string weather, Move move, StattedPokemon defendingPokemon)
        {
            double multiplier = 1;
            var effectiveness = Weather.EffectivenessChart[weather];
            var isBoostMove = effectiveness.AdditionalBoostedMoves.Contains(move.Name);
            if (effectiveness.BoostedOffensiveTypes.Contains(move.Type.Name) || isBoostMove)
            {
                multiplier = 1.5;
            }
            else if (effectiveness.WeakenedOffensiveTypes.Contains(move.Type.Name))
            {
                multiplier = 0.5;
            }

            return multiplier;
        }

        private static double GetWeatherDefenseMultiplier(string weather, Move move, StattedPokemon defendingPokemon)
        {
            double multiplier = 1;
            var effectiveness = Weather.EffectivenessChart[weather];
            var isBoostDefense = false;
            var checkForBoostedDefense =
                (weather == Weather.Hail && move.DamageClass.Name == DamageClasses.Physical) ||
                (weather == Weather.Sandstorm && move.DamageClass.Name == DamageClasses.Special);
            if (checkForBoostedDefense)
            {
                isBoostDefense = defendingPokemon.Types.Any(effectiveness.BoostedDefensiveTypes.Contains);
            }
            if (isBoostDefense)
            {
                multiplier = 1.5;
            }

            return multiplier;
        }

        private static double GetTerrainMultiplier(string terrain, Move move, StattedPokemon offendingPokemon, StattedPokemon defendingPokemon)
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
            var isBoostMove = effectiveness.AdditionalBoostedMoves.Contains(move.Name);
            if (effectiveness.BoostedOffensiveTypes.Contains(move.Type.Name) || isBoostMove)
            {
                multiplier = 1.5;
            }
            else if (effectiveness.WeakenedOffensiveTypes.Contains(move.Type.Name))
            {
                multiplier = 0.5;
            }

            return multiplier;
        }
    }
}
