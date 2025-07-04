﻿namespace TheReplacement.PokemonDamageCalc.Domain
{
    using TheReplacement.PokemonDamageCalc.Constants;
    using TheReplacement.PokemonDamageCalc.DataModel;
    using TheReplacement.PokemonDamageCalc.DTOs;
    using TheReplacement.PokemonDamageCalc.Services;

    public class DamageService(IStatService statService) : IDamageService
    {
        public IEnumerable<DamageRoll> GetDamageRolls(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move,
            Conditionals conditionals,
            StringPair statusConditions,
            string weather,
            string terrain)
        {
            conditionals.IsCriticalHit = conditionals.IsCriticalHit && (!Collections.IgnoreCriticals.Contains(defensivePokemon.Ability) || Collections.IgnoreAbilities.Contains(offensivePokemon.Ability));
            var typeEffectivenessMultiplier = GetTypeEffectivenessMultiplier(move.Type, defensivePokemon);
            var (attack, defense) = GetStats(statService, offensivePokemon, defensivePokemon, move, conditionals.IsCriticalHit);
            var skipDamage = SkipDamageCalculation(
                offensivePokemon,
                defensivePokemon,
                move,
                conditionals,
                attack,
                typeEffectivenessMultiplier);
            if (skipDamage)
            {
                return [];
            }
            var trueDamage = GetTrueDamage(offensivePokemon, move);
            if (trueDamage != null)
            {
                return Enumerable.Range(85, 16).Select(roll => new DamageRoll
                {
                    Roll = roll,
                    Damage = trueDamage.Value
                });
            }
            var (weatherMultiplier, weatherDefenseMultiplier) = GetWeatherMultipliers(offensivePokemon, defensivePokemon, weather, move);
            defense *= weatherDefenseMultiplier;
            var basePower = GetMovePower(offensivePokemon, defensivePokemon, move);
            var targetsMultiplier = GetTargetsMultiplier(conditionals, move);
            var parentalBondMultiplier = GetParentalBondMultiplier(offensivePokemon);
            var glaiveRushMultiplier = GetGlaiveRushMultiplier(conditionals);
            var critMultiplier = GetCriticalMuliplier(offensivePokemon, defensivePokemon, move, statusConditions.Defensive, conditionals);
            var stab = GetStab(offensivePokemon, move);
            var statusMultiplier = GetStatusMultiplier(offensivePokemon, move, statusConditions.Offensive);
            var terrainMultiplier = GetTerrainMultiplier(terrain, move, offensivePokemon);
            var otherMultiplier = GetOtherMultiplier(offensivePokemon, defensivePokemon, move, conditionals, typeEffectivenessMultiplier);
            var rawDamage = Math.Floor((2 * offensivePokemon.Level / 5.0 + 2) * basePower * (attack / defense) / 50 + 2);
            rawDamage = Math.Floor(rawDamage * targetsMultiplier);
            rawDamage = Math.Floor(rawDamage * parentalBondMultiplier);
            rawDamage = Math.Floor(rawDamage * weatherMultiplier);
            rawDamage = Math.Floor(rawDamage * glaiveRushMultiplier);
            rawDamage = Math.Floor(rawDamage * critMultiplier);
            rawDamage = Math.Floor(rawDamage * stab);
            rawDamage = Math.Floor(rawDamage * typeEffectivenessMultiplier);
            rawDamage = Math.Floor(rawDamage * statusMultiplier);
            rawDamage = Math.Floor(rawDamage * otherMultiplier);
            rawDamage = Math.Floor(rawDamage * terrainMultiplier);
            return Enumerable.Range(85, 16).Select(roll => new DamageRoll
            {
                Roll = roll,
                Damage = Math.Max(Math.Floor(rawDamage * (roll / 100.0)), 1)
            });
        }

        #region Helper functions
        private static bool SkipDamageCalculation(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move,
            Conditionals conditionals,
            double attack,
            double typeEffectivenessMultiplier)
        {
            return attack == 0 ||
                typeEffectivenessMultiplier == 0 ||
                (Collections.SoundMoves.Contains(move.Name) && defensivePokemon.Ability == Abilities.Soundproof && !Collections.IgnoreAbilities.Contains(offensivePokemon.Ability)) ||
                (Collections.BulletMoves.Contains(move.Name) && defensivePokemon.Ability == Abilities.Bulletproof && !Collections.IgnoreAbilities.Contains(offensivePokemon.Ability)) ||
                (conditionals.ProtectActive && !(Collections.IgnoreProtectMoves.Contains(move.Name) || offensivePokemon.Ability == Abilities.UnseenFist)) ||
                (conditionals.SubstituteActive && !(Collections.SoundMoves.Contains(move.Name) || Collections.IgnoreSubstitute.Contains(move.Name) || offensivePokemon.Ability == Abilities.Infiltrator)) ||
                (conditionals.UsedDig && !Collections.DigMultipliers.Contains(move.Name)) ||
                (conditionals.UsedDive && !Collections.DiveMultipliers.Contains(move.Name)) ||
                (conditionals.UsedFly && !Collections.IgnoreFly.Contains(move.Name));

        }

        private static int? GetTrueDamage(
            StattedPokemon offensivePokemon,
            MoveData move)
        {
            if (Maps.TrueDamageMap.TryGetValue(move.Name, out var func))
            {
                return func(offensivePokemon);
            }

            return null;
        }

        private static (double EffectiveAttackStat, double EffectiveDefenseStat) GetStats(
            IStatService statService,
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move,
            bool isCritical)
        {
            double attack = 0, defense = 0;
            double attackMultiplier = 1;
            if (Maps.IsStatBoostByAbilityMap.TryGetValue(offensivePokemon.Ability, out var func1) && func1(offensivePokemon, move))
            {
                attackMultiplier *= 1.5;
            }
            if (Maps.IsStatBoostByChoiceItemMap.TryGetValue(offensivePokemon.HeldItem, out var func2) && func2(move))
            {
                attackMultiplier *= 1.5;
            }
            else if (Maps.OtherStatBoostingItemMap.TryGetValue(offensivePokemon.HeldItem, out var itemMultiplier))
            {
                attackMultiplier *= itemMultiplier;
            }
            if (move.DamageClass == DamageClasses.Physical)
            {
                var offensiveAttack = statService.GetAttack(isCritical, defensivePokemon.Ability == Abilities.Unaware, offensivePokemon) * attackMultiplier;
                var offensiveDefense = statService.GetDefenseForBodyPress(isCritical, defensivePokemon.Ability == Abilities.Unaware, offensivePokemon);
                attack = move.Name == Moves.BodyPress && offensiveDefense > offensiveAttack
                    ? offensiveDefense
                    : move.Name == Moves.FoulPlay
                        ? statService.GetAttack(isCritical, defensivePokemon.Ability == Abilities.Unaware, defensivePokemon) * attackMultiplier
                        : offensiveAttack;
                defense = statService.GetDefense(isCritical, offensivePokemon.Ability == Abilities.Unaware || move.Name == Moves.SacredSword, defensivePokemon);
            }
            else if (move.DamageClass == DamageClasses.Special)
            {
                attack = statService.GetSpecialAttack(isCritical, defensivePokemon.Ability == Abilities.Unaware, offensivePokemon) * attackMultiplier;
                defense = Collections.SpecialMovesThatDoPhysicalDamage.Contains(move.Name)
                    ? statService.GetDefense(isCritical, offensivePokemon.Ability == Abilities.Unaware, defensivePokemon)
                    : statService.GetSpecialDefense(isCritical, offensivePokemon.Ability == Abilities.Unaware, defensivePokemon);
            }

            return (attack, defense);
        }

        private static (double OffensiveMultipler, double DefensiveMultiplier) GetWeatherMultipliers(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            string weather,
            MoveData move)
        {
            var offensiveMultiplier = GetWeatherMultiplier(offensivePokemon, defensivePokemon, weather, move);
            var defensiveMultiplier = GetWeatherDefenseMultiplier(offensivePokemon, defensivePokemon, weather, move);
            return (offensiveMultiplier, defensiveMultiplier);
        }

        private static double GetWeatherDefenseMultiplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            string weather,
            MoveData move)
        {
            double multiplier = 1;
            if (Collections.IgnoreWeather.Contains(offensivePokemon.Ability) || Collections.IgnoreWeather.Contains(defensivePokemon.Ability))
            {
                return multiplier;
            }
            var effectiveness = Maps.WeatherEffectivenessChart[weather];
            var isBoostDefense = false;
            var checkForBoostedDefense =
                (weather == Weather.Hail && move.DamageClass == DamageClasses.Physical) ||
                (weather == Weather.Sandstorm && move.DamageClass == DamageClasses.Special);
            var types = GetPokemonTypes(defensivePokemon);
            if (checkForBoostedDefense)
            {
                isBoostDefense = types.Any(effectiveness.BoostedDefensiveTypes.Contains);
            }
            if (isBoostDefense)
            {
                multiplier = 1.5;
            }

            return multiplier;
        }

        private static int GetMovePower(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move)
        {
            var power = move.BasePower;
            if (Maps.ConditionalMovePowerMap.TryGetValue(move.Name, out var powerFunction))
            {
                power = powerFunction(offensivePokemon, defensivePokemon, move);
            }
            if (power == null)
            {
                return 0;
            }
            if (Collections.SoundMoves.Contains(move.Name) && offensivePokemon.Ability == Abilities.PunkRock)
            {
                power = (int)(power * 1.3);
            }
            return power ?? 0;
        }

        private static double GetTargetsMultiplier(
            Conditionals conditionals,
            MoveData move)
        {
            if (Collections.MultiTargetMoves.Contains(move.Target) && conditionals.IsTagBattle)
            {
                return 0.75;
            }

            return 1;
        }

        private static double GetWeatherMultiplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            string weather,
            MoveData move)
        {
            double multiplier = 1;
            if (Collections.IgnoreWeather.Contains(offensivePokemon.Ability) || Collections.IgnoreWeather.Contains(defensivePokemon.Ability))
            {
                return multiplier;
            }
            var effectiveness = Maps.WeatherEffectivenessChart[weather];
            var isBoostMove = effectiveness.AdditionalBoostedMoves.Contains(move.Name);
            if (effectiveness.BoostedOffensiveTypes.Contains(move.Type) || isBoostMove)
            {
                multiplier = 1.5;
            }
            else if (effectiveness.WeakenedOffensiveTypes.Contains(move.Type))
            {
                multiplier = 0.5;
            }

            return multiplier;
        }

        private static double GetCriticalMuliplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move,
            string defensiveStatusCondition,
            Conditionals conditionals)
        {
            double criticalMultiplier = 1;
            if (Collections.IgnoreCriticals.Contains(defensivePokemon.Ability) && !Collections.IgnoreAbilities.Contains(offensivePokemon.Ability))
            {
                return criticalMultiplier;
            }
            var isGuarantee = IsGuaranteedCritical(offensivePokemon, move, defensiveStatusCondition, conditionals.IsLaserFocus);
            if (isGuarantee || conditionals.IsCriticalHit)
            {
                criticalMultiplier = offensivePokemon.Ability == Abilities.Adaptability
                    ? 2
                    : 1.5;
            }

            return criticalMultiplier;
        }

        private static bool IsGuaranteedCritical(
            StattedPokemon offensivePokemon,
            MoveData move,
            string defensiveStatusCondition,
            bool isLaserFocus)
        {
            return Collections.AlwaysCritical.Contains(move.Name) ||
                ((offensivePokemon.Ability == Abilities.Merciless) && (defensiveStatusCondition == Statuses.Poisoned || defensiveStatusCondition == Statuses.BadlyPoisoned)) ||
                isLaserFocus;
        }

        private static double GetOtherMultiplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            MoveData move,
            Conditionals conditionals,
            double typeEffectivenessMultiplier)
        {
            double multiplier = 1;
            if (conditionals.IsMinimized && Collections.MinimizedMultipliers.Contains(move.Name))
            {
                multiplier *= 2;
            }
            if (conditionals.UsedDig)
            {
                if (Collections.DigMultipliers.Contains(move.Name))
                {
                    multiplier *= 2;
                }
                else
                {
                    return 0;
                }
            }
            if (conditionals.UsedDive)
            {
                if (Collections.DiveMultipliers.Contains(move.Name))
                {
                    multiplier *= 2;
                }
                else
                {
                    return 0;
                }
            }
            if ((conditionals.UsedReflect || conditionals.UsedAuroraVeil) && move.DamageClass == DamageClasses.Physical)
            {
                multiplier /= 2;
            }
            if ((conditionals.UsedLightScreen || conditionals.UsedAuroraVeil) && move.DamageClass == DamageClasses.Special)
            {
                multiplier /= 2;
            }
            if (typeEffectivenessMultiplier > 1 && Collections.BikeSignatures.Contains(move.Name))
            {
                multiplier *= 5461.0 / 4096;
            }
            if (Collections.HalfAtFullHealth.Contains(defensivePokemon.Ability) && defensivePokemon.IsAtFullHealth)
            {
                multiplier /= 2;
            }
            // pokeapi doesn't track contact or sound moves :(
            if (Collections.ContactMoves.Contains(move.Name) && defensivePokemon.Ability == Abilities.Fluffy)
            {
                multiplier /= 2;
            }
            if (Collections.SoundMoves.Contains(move.Name) && defensivePokemon.Ability == Abilities.PunkRock)
            {
                multiplier /= 2;
            }
            if (Collections.SoundMoves.Contains(move.Name) && offensivePokemon.Ability == Abilities.PunkRock)
            {
                multiplier *= 1.3;
            }
            if (defensivePokemon.Ability == Abilities.IceScales && move.DamageClass == DamageClasses.Special)
            {
                multiplier /= 2;
            }
            if (conditionals.IsFriendGuarded)
            {
                multiplier *= 0.75;
            }
            if (Collections.DefendedSuperEffective.Contains(defensivePokemon.Ability) && typeEffectivenessMultiplier > 1)
            {
                multiplier *= 0.75;
            }
            if (offensivePokemon.Ability == Abilities.Neuroforce && typeEffectivenessMultiplier > 1)
            {
                multiplier *= 1.25;
            }
            if (offensivePokemon.Ability == Abilities.Sniper && conditionals.IsCriticalHit)
            {
                multiplier *= 1.5;
            }
            if (offensivePokemon.Ability == Abilities.TintedLens && typeEffectivenessMultiplier < 1)
            {
                multiplier *= 2;
            }
            if (defensivePokemon.Ability == Abilities.Fluffy && move.Type == Types.Fire)
            {
                multiplier *= 2;
            }
            if (typeEffectivenessMultiplier > 1 && Maps.TypeResistantBerryItemMap.TryGetValue(defensivePokemon.HeldItem, out var berryResistantType) && move.Type == berryResistantType)
            {
                if (defensivePokemon.Ability == Abilities.Ripen)
                {
                    multiplier *= 0.25;
                }
                else
                {
                    multiplier *= 0.5;
                }
            }
            if (typeEffectivenessMultiplier > 1 && offensivePokemon.HeldItem == Items.ExpertBelt)
            {
                multiplier *= 4915.0 / 4096;
            }
            if (typeEffectivenessMultiplier > 1 && offensivePokemon.HeldItem == Items.LifeOrb)
            {
                multiplier *= 5324.0 / 4096;
            }
            // todo: metronome
            return multiplier;
        }

        private static double GetParentalBondMultiplier(StattedPokemon offensivePokemon)
        {
            if (offensivePokemon.Ability == Abilities.ParentalBond)
            {
                return 1.25;
            }

            return 1;
        }

        private static int GetGlaiveRushMultiplier(Conditionals conditionals)
        {
            if (conditionals.AfterGlaiveRush)
            {
                return 2;
            }

            return 1;
        }

        private static double GetStab(StattedPokemon offendingPokemon, MoveData move)
        {
            double stabMultiplier = 1;
            var types = offendingPokemon.Types;
            var typeCount = offendingPokemon.Types.Append(offendingPokemon.TeraType).Count(x => x == move.Type);
            if (types.Contains(move.Type))
            {
                if (offendingPokemon.TeraType == move.Type)
                {
                    if (offendingPokemon.Ability == Abilities.Adaptability)
                    {
                        stabMultiplier = 2.25;
                    }
                    else
                    {
                        stabMultiplier = 2;
                    }
                }

                stabMultiplier = 1.5;
            }
            else if (offendingPokemon.TeraType == move.Type)
            {
                stabMultiplier = 1.5;
            }
            if (IsSteelWorker(offendingPokemon, move))
            {
                stabMultiplier *= 1.5;
            }

            return stabMultiplier;
        }

        private static bool IsSteelWorker(StattedPokemon offendingPokemon, MoveData move)
        {
            return move.Type == Types.Steel && offendingPokemon.Ability == Abilities.SteelWorker;
        }

        private static double GetTypeEffectivenessMultiplier(
            string moveTypeName,
            StattedPokemon defendingPokemon,
            bool ignoreResistances = false,
            bool ignoreImmunities = false)
        {
            double effectiveness = 1;
            var types = GetPokemonTypes(defendingPokemon);
            var pokemonTypes = types.Select(x => Maps.TypeEffectivenessChart[x]);
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

        private static double GetStatusMultiplier(
            StattedPokemon offendingPokemon,
            MoveData move,
            string status)
        {
            double statusMultiplier = 1;
            if (move.DamageClass != DamageClasses.Physical)
            {
                return statusMultiplier;
            }
            if (offendingPokemon.Ability == Abilities.Guts && status != Statuses.Healthy)
            {
                statusMultiplier = move.Name == Moves.Facade
                    ? 3
                    : 1.5;
            }
            else if (move.Name == Moves.Facade && status != Statuses.Healthy)
            {
                statusMultiplier = 2;
            }
            else if (offendingPokemon.Ability == Abilities.ToxicBoost && status == Statuses.Poisoned)
            {
                statusMultiplier = move.Name == Moves.Facade
                    ? 3
                    : 1.5;
            }
            else if (status == Statuses.Burn)
            {
                statusMultiplier = 0.5;
            }

            return statusMultiplier;
        }

        private static double GetTerrainMultiplier(
            string terrain,
            MoveData move,
            StattedPokemon offendingPokemon)
        {
            double multiplier = 1;
            var types = offendingPokemon.Types.Append(offendingPokemon.TeraType).Where(x => x != null);
            if (offendingPokemon.Ability == Abilities.Levitate || types.Any(type => type == Types.Flying))
            {
                if (!(offendingPokemon.Ability == Abilities.QuarkDrive && terrain == Terrain.ElectricTerrain))
                {
                    return multiplier;
                }
            }
            var effectiveness = Maps.TerrainEffectivenessChart[terrain];
            var isBoostMove = effectiveness.AdditionalBoostedMoves.Contains(move.Name);
            if (effectiveness.BoostedOffensiveTypes.Contains(move.Type) || isBoostMove)
            {
                multiplier = 1.5;
            }
            else if (effectiveness.WeakenedOffensiveTypes.Contains(move.Type))
            {
                multiplier = 0.5;
            }

            return multiplier;
        }

        private static IEnumerable<string> GetPokemonTypes(StattedPokemon pokemon)
        {
            return pokemon.TeraType == null
                ? pokemon.Types
                : [pokemon.TeraType];
        }
        #endregion
    }
}
