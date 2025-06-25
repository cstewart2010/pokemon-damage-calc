using PokeApiNet;
using System.Collections.ObjectModel;
using TheReplacement.PokemonDamageCalc.Client.Constants;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public static class DamageHelper
    {
        private static readonly ICollection<string>
            SpecialMovesThatDoPhysicalDamage = [Moves.Psyshock, Moves.Psystrike, Moves.SecretSword],
            AlwaysCritical = [],
            IgnoreCriticals = [Abilities.BattleArmor, Abilities.ShellArmor],
            IgnoreAbilities = [Abilities.MoldBreaker, Abilities.Teravolt, Abilities.Turboblaze],
            IgnoreWeather = [Abilities.AirLock, Abilities.CloudNine],
            MinimizedMultipliers = [Moves.BodySlam, Moves.Stomp, Moves.DragonRush, Moves.HeatCrash, Moves.HeavySlam, Moves.FlyingPress, Moves.SupercellSlam],
            DigMultipliers = [Moves.Magnitude, Moves.Earthquake],
            DiveMultipliers = [Moves.Surf, Moves.Whirlpool],
            IgnoreFly = [Moves.Gust, Moves.Hurricane, Moves.SkyUppercut, Moves.SmackDown, Moves.ThousandArrows, Moves.Thunder, Moves.Twister],
            BikeSignatures = [Moves.CollisionCourse, Moves.ElectroDrift],
            HalfAtFullHealth = [Abilities.Multiscale, Abilities.ShadowShield],
            ContactMoves = [],
            SoundMoves = [],
            MultiTargetMoves = [MoveTargets.AllPokemon, MoveTargets.AllOtherPokemon],
            DefendedSuperEffective = [Abilities.Filter, Abilities.PrismArmor, Abilities.SolidRock],
            IgnoreProtectMoves = [Moves.DoomDesire, Moves.Feint, Moves.FutureSight, Moves.HyperDrill, Moves.HyperspaceFury, Moves.HyperspaceHole, Moves.HyperspaceHole, Moves.MightyCleave, Moves.PhantomForce, Moves.ShadowForce],
            IgnoreSubstitute = [Moves.HyperspaceFury, Moves.HyperspaceHole, Moves.SpectralThief];

        private static readonly ReadOnlyDictionary<string, Func<StattedPokemon, int>> TrueDamageMap;
        private static readonly ReadOnlyDictionary<string, Func<StattedPokemon, Move, bool>> IsStatBoostByAbilityMap;
        private static readonly ReadOnlyDictionary<string, Func<Move, bool>> IsStatBoostByChoiceItemMap;
        private static readonly ReadOnlyDictionary<string, double> OtherStatBoostingItemMap;
        private static readonly ReadOnlyDictionary<string, string> TypeResistantBerryItemMap;
        private static readonly ReadOnlyDictionary<string, Func<StattedPokemon, StattedPokemon, Move, int>> ConditionalMovePowerMap;

        static DamageHelper()
        {
            var damageDictionary = new Dictionary<string, Func<StattedPokemon, int>>
            {
                {Moves.SonicBoom, (offensivePokemon) => 20},
                {Moves.DragonRage, (offensivePokemon) => 40},
                {Moves.SeismicToss, (offensivePokemon) => offensivePokemon.Level },
                {Moves.NightShade, (offensivePokemon) => offensivePokemon.Level },
            };
            var statBoostDictionary = new Dictionary<string, Func<StattedPokemon, Move, bool>>
            {
                {Abilities.Overgrow, GetIsStatBoostByAbilityFunction(Types.Grass) },
                {Abilities.Blaze, GetIsStatBoostByAbilityFunction(Types.Fire) },
                {Abilities.Torrent, GetIsStatBoostByAbilityFunction(Types.Water) },
                {Abilities.Swarm, GetIsStatBoostByAbilityFunction(Types.Bug) },
            };

            var choiceBoostDictionary = new Dictionary<string, Func<Move, bool>>
            {
                {Items.ChoiceBand, GetIsStatBoostByChoiceItemFunction(DamageClasses.Physical) },
                {Items.ChoiceSpecs, GetIsStatBoostByChoiceItemFunction(DamageClasses.Special) },
            };

            // todo: rest of stat boosting items
            var otherItemDictionary = new Dictionary<string, double>
            {
                {Items.SilkScarf, 1.2 },
            };

            var typeResistantBerries = new Dictionary<string, string>
            {
                { Items.BabiriBerry, Types.Steel },
                { Items.ChartiBerry, Types.Rock },
                { Items.ChilanBerry, Types.Normal },
                { Items.ChopleBerry, Types.Fighting },
                { Items.CobaBerry, Types.Flying },
                { Items.ColburBerry, Types.Dark },
                { Items.HabanBerry, Types.Dragon },
                { Items.KasibBerry, Types.Ghost },
                { Items.KebiaBerry, Types.Poison },
                { Items.OccaBerry, Types.Fire },
                { Items.PasshoBerry, Types.Water },
                { Items.PayapaBerry, Types.Psychic },
                { Items.RindoBerry, Types.Grass },
                { Items.RoseliBerry, Types.Fairy },
                { Items.ShucaBerry, Types.Ground },
                { Items.TangaBerry, Types.Bug },
                { Items.WacanBerry, Types.Electric },
                { Items.YacheBerry, Types.Ice },
            };

            // todo: https://bulbapedia.bulbagarden.net/wiki/Category:Moves_that_have_variable_power
            var powerMap = new Dictionary<string, Func<StattedPokemon, StattedPokemon, Move, int>>
            {
                {Moves.Return, (offensivePokemon, _, _) => Math.Max((int)(offensivePokemon.Friendship/2.5),1) },
                {Moves.Frustation, (offensivePokemon, _, _) => Math.Max((int)((255-offensivePokemon.Friendship)/2.5),1) },
                {Moves.HeavySlam, (offensivePokemon, defensivePokemon, _) => HeavySlamMovePower(offensivePokemon, defensivePokemon) },
                {Moves.HeatCrash, (offensivePokemon, defensivePokemon, _) => HeavySlamMovePower(offensivePokemon, defensivePokemon) },
                {Moves.ElectroBall, (offensivePokemon, defensivePokemon, _) => GetElectroBallMovePower(offensivePokemon, defensivePokemon) },
                {Moves.GyroBall, (offensivePokemon, defensivePokemon, _) => GetGyroBallMovePower(offensivePokemon, defensivePokemon) },
            };

            TrueDamageMap = damageDictionary.AsReadOnly();
            IsStatBoostByAbilityMap = statBoostDictionary.AsReadOnly();
            IsStatBoostByChoiceItemMap = choiceBoostDictionary.AsReadOnly();
            OtherStatBoostingItemMap = otherItemDictionary.AsReadOnly();
            TypeResistantBerryItemMap = typeResistantBerries.AsReadOnly();
            ConditionalMovePowerMap = powerMap.AsReadOnly();
        }

        public static IEnumerable<DamageRoll> GetDamageRolls(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            Conditionals conditionals,
            StringPair statusConditions,
            string weather,
            string terrain)
        {
            conditionals.IsCriticalHit &= !IgnoreCriticals.Contains(defensivePokemon.Ability) || IgnoreAbilities.Contains(offensivePokemon.Ability);
            var typeEffectivenessMultiplier = GetTypeEffectivenessMultiplier(move.Type.Name, defensivePokemon);
            var (attack, defense) = GetStats(offensivePokemon, defensivePokemon, move, conditionals.IsCriticalHit);
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
            var trueDamage = GetTrueDamage(offensivePokemon, defensivePokemon, move);
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
            var terrainMultiplier = GetTerrainMultiplier(terrain, move, offensivePokemon, defensivePokemon);
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

        private static Func<StattedPokemon, Move, bool> GetIsStatBoostByAbilityFunction(string type)
        {
            return (offensivePokemon, move) => move.Type.Name == type && offensivePokemon.HP >= 2 * offensivePokemon.CurrentHP;
        }

        private static Func<Move, bool> GetIsStatBoostByChoiceItemFunction(string damamgeClass)
        {
            return (move) => move.DamageClass.Name == damamgeClass;
        }

        private static int HeavySlamMovePower(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon)
        {
            var ratio = (1.0 * defensivePokemon.Weight) / offensivePokemon.Weight;
            if (ratio > 0.5)
            {
                return 40;
            }
            if (ratio > 0.3334)
            {
                return 60;
            }
            if (ratio > 0.25)
            {
                return 80;
            }
            if (ratio > 0.2)
            {
                return 100;
            }

            return 120;
        }

        private static int GetElectroBallMovePower(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon)
        {
            var ratio = defensivePokemon.Speed / offensivePokemon.Speed;
            if (ratio > 1 || ratio == 0)
            {
                return 40;
            }
            if (ratio > 0.5)
            {
                return 60;
            }
            if (ratio > 0.3333)
            {
                return 80;
            }
            if (ratio > 0.25)
            {
                return 100;
            }

            return 120;
        }

        private static int GetGyroBallMovePower(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon)
        {
            // factor in speed altering conditions
            if (offensivePokemon.Speed == 0)
            {
                return 1;
            }

            return Math.Min(150, (int)(25 * defensivePokemon.Speed / offensivePokemon.Speed) + 1);
        }

        private static bool SkipDamageCalculation(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            Conditionals conditionals,
            double attack,
            double typeEffectivenessMultiplier)
        {
            return attack == 0 ||
                conditionals.UsedFly ||
                typeEffectivenessMultiplier == 0 ||
                (conditionals.ProtectActive && !(IgnoreProtectMoves.Contains(move.Name) || offensivePokemon.Ability == Abilities.UnseenFist)) ||
                (conditionals.SubstituteActive && !(SoundMoves.Contains(move.Name) || IgnoreSubstitute.Contains(move.Name) || offensivePokemon.Ability == Abilities.Infiltrator)) ||
                (conditionals.UsedDig && !DigMultipliers.Contains(move.Name)) ||
                (conditionals.UsedFly && !DiveMultipliers.Contains(move.Name)) ||
                (conditionals.UsedDive && !IgnoreFly.Contains(move.Name));

        }

        private static int? GetTrueDamage(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move)
        {
            if (TrueDamageMap.TryGetValue(move.Name, out var func))
            {
                return func(offensivePokemon);
            }

            return null;
        }

        private static (double EffectiveAttackStat, double EffectiveDefenseStat) GetStats(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            bool isCritical)
        {
            double attack = 0, defense = 0;
            double attackMultiplier = 1;
            // todo: choice band/specs
            if (IsStatBoostByAbilityMap.TryGetValue(offensivePokemon.Ability, out var func1) && func1(offensivePokemon, move))
            {
                attackMultiplier *= 1.5;
            }
            if (IsStatBoostByChoiceItemMap.TryGetValue(offensivePokemon.HeldItem, out var func2) && func2(move))
            {
                attackMultiplier *= 1.5;
            }
            else if (OtherStatBoostingItemMap.TryGetValue(offensivePokemon.HeldItem, out var itemMultiplier))
            {
                attackMultiplier *= itemMultiplier;
            }
            if (move.DamageClass.Name == DamageClasses.Physical)
            {
                var offensiveAttack = StatHelper.GetAttack(isCritical, defensivePokemon.Ability == Abilities.Unaware, offensivePokemon) * attackMultiplier;
                var offensiveDefense = StatHelper.GetDefenseForBodyPress(isCritical, defensivePokemon.Ability == Abilities.Unaware, offensivePokemon);
                attack = move.Name == Moves.BodyPress && offensiveDefense > offensiveAttack
                    ? offensiveDefense
                    : move.Name == Moves.FoulPlay
                        ? StatHelper.GetAttack(isCritical, defensivePokemon.Ability == Abilities.Unaware, defensivePokemon) * attackMultiplier
                        : offensiveAttack;
                defense = StatHelper.GetDefense(isCritical, offensivePokemon.Ability == Abilities.Unaware || move.Name == Moves.SacredSword, defensivePokemon);
            }
            else if (move.DamageClass.Name == DamageClasses.Special)
            {
                attack = StatHelper.GetSpecialAttack(isCritical, defensivePokemon.Ability == Abilities.Unaware, offensivePokemon) * attackMultiplier;
                defense = SpecialMovesThatDoPhysicalDamage.Contains(move.Name)
                    ? StatHelper.GetDefense(isCritical, offensivePokemon.Ability == Abilities.Unaware, defensivePokemon)
                    : StatHelper.GetSpecialDefense(isCritical, offensivePokemon.Ability == Abilities.Unaware, defensivePokemon);
            }

            return (attack, defense);
        }

        private static (double OffensiveMultipler, double DefensiveMultiplier) GetWeatherMultipliers(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            string weather,
            Move move)
        {
            var offensiveMultiplier = GetWeatherMultiplier(offensivePokemon, defensivePokemon, weather, move);
            var defensiveMultiplier = GetWeatherDefenseMultiplier(offensivePokemon, defensivePokemon, weather, move);
            return (offensiveMultiplier, defensiveMultiplier);
        }

        private static double GetWeatherDefenseMultiplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            string weather,
            Move move)
        {
            double multiplier = 1;
            if (IgnoreWeather.Contains(offensivePokemon.Ability) || IgnoreWeather.Contains(defensivePokemon.Ability))
            {
                return multiplier;
            }
            var effectiveness = Weather.EffectivenessChart[weather];
            var isBoostDefense = false;
            var checkForBoostedDefense =
                (weather == Weather.Hail && move.DamageClass.Name == DamageClasses.Physical) ||
                (weather == Weather.Sandstorm && move.DamageClass.Name == DamageClasses.Special);
            if (checkForBoostedDefense)
            {
                isBoostDefense = defensivePokemon.Types.Any(effectiveness.BoostedDefensiveTypes.Contains);
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
            Move move)
        {
            // todo: move power boosting items
            var power = move.Power;
            if (ConditionalMovePowerMap.TryGetValue(move.Name, out var powerFunction))
            {
                power = powerFunction(offensivePokemon, defensivePokemon, move);
            }
            if (power == null)
            {
                return 0;
            }
            if (SoundMoves.Contains(move.Name) && offensivePokemon.Ability == Abilities.PunkRock)
            {
                power = (int)(power * 1.3);
            }
            return power ?? 0;
        }

        private static double GetTargetsMultiplier(
            Conditionals conditionals,
            Move move)
        {
            if (MultiTargetMoves.Contains(move.Target.Name) && conditionals.IsTagBattle)
            {
                return 0.75;
            }

            return 1;
        }

        private static double GetParentalBondMultiplier(StattedPokemon offensivePokemon)
        {
            if (offensivePokemon.Ability == Abilities.ParentalBond)
            {
                return 1.25;
            }

            return 1;
        }

        private static double GetWeatherMultiplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            string weather,
            Move move)
        {
            double multiplier = 1;
            if (IgnoreWeather.Contains(offensivePokemon.Ability) || IgnoreWeather.Contains(defensivePokemon.Ability))
            {
                return multiplier;
            }
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

        private static int GetGlaiveRushMultiplier(Conditionals conditionals)
        {
            if (conditionals.AfterGlaiveRush)
            {
                return 2;
            }

            return 1;
        }

        private static double GetCriticalMuliplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            string defensiveStatusCondition,
            Conditionals conditionals)
        {
            double criticalMultiplier = 1;
            if (IgnoreCriticals.Contains(defensivePokemon.Ability) && !IgnoreAbilities.Contains(offensivePokemon.Ability))
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
            Move move,
            string defensiveStatusCondition,
            bool isLaserFocus)
        {
            return AlwaysCritical.Contains(move.Name) ||
                ((offensivePokemon.Ability == Abilities.Merciless) && (defensiveStatusCondition == Statuses.Poisoned || defensiveStatusCondition == Statuses.BadlyPoisoned)) ||
                isLaserFocus;
        }

        private static double GetStab(StattedPokemon offendingPokemon, Move move)
        {
            if (offendingPokemon.Types.Contains(move.Type.Name) || IsSteelWorker(offendingPokemon, move))
            {
                if (offendingPokemon.Ability == Abilities.Adaptability)
                {
                    return 2;
                }

                return 1.5;
            }

            return 1;
        }

        private static bool IsSteelWorker(StattedPokemon offendingPokemon, Move move)
        {
            return move.Type.Name == Types.Steel && offendingPokemon.Ability == Abilities.SteelWorker;
        }

        private static double GetTypeEffectivenessMultiplier(string moveTypeName, StattedPokemon defendingPokemon, bool ignoreResistances = false, bool ignoreImmunities = false)
        {
            double effectiveness = 1;

            var pokemonTypes = defendingPokemon.Types.Select(x => Types.EffectivenessChart[x]);
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

        private static double GetStatusMultiplier(StattedPokemon offendingPokemon, Move move, string status)
        {
            double statusMultiplier = 1;
            if (move.DamageClass.Name != DamageClasses.Physical)
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

        private static double GetOtherMultiplier(
            StattedPokemon offensivePokemon,
            StattedPokemon defensivePokemon,
            Move move,
            Conditionals conditionals,
            double typeEffectivenessMultiplier)
        {
            double multiplier = 1;
            if (conditionals.IsMinimized && MinimizedMultipliers.Contains(move.Name))
            {
                multiplier *= 2;
            }
            if (conditionals.UsedDig)
            {
                if (DigMultipliers.Contains(move.Name))
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
                if (DiveMultipliers.Contains(move.Name))
                {
                    multiplier *= 2;
                }
                else
                {
                    return 0;
                }
            }
            if ((conditionals.UsedReflect || conditionals.UsedAuroraVeil) && move.DamageClass.Name == DamageClasses.Physical)
            {
                multiplier /= 2;
            }
            if ((conditionals.UsedLightScreen || conditionals.UsedAuroraVeil) && move.DamageClass.Name == DamageClasses.Special)
            {
                multiplier /= 2;
            }
            if (typeEffectivenessMultiplier > 1 && BikeSignatures.Contains(move.Name))
            {
                multiplier *= 5461.0 / 4096;
            }
            if (HalfAtFullHealth.Contains(defensivePokemon.Ability) && defensivePokemon.IsAtFullHealth)
            {
                multiplier /= 2;
            }
            // pokeapi doesn't track contact or sound moves :(
            if (ContactMoves.Contains(move.Name) && defensivePokemon.Ability == Abilities.Fluffy)
            {
                multiplier /= 2;
            }
            if (SoundMoves.Contains(move.Name) && defensivePokemon.Ability == Abilities.PunkRock)
            {
                multiplier /= 2;
            }
            if (SoundMoves.Contains(move.Name) && offensivePokemon.Ability == Abilities.PunkRock)
            {
                multiplier *= 1.3;
            }
            if (defensivePokemon.Ability == Abilities.IceScales && move.DamageClass.Name == DamageClasses.Special)
            {
                multiplier /= 2;
            }
            if (conditionals.IsFriendGuarded)
            {
                multiplier *= 0.75;
            }
            if (DefendedSuperEffective.Contains(defensivePokemon.Ability) && typeEffectivenessMultiplier > 1)
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
            if (defensivePokemon.Ability == Abilities.Fluffy && move.Type.Name == Types.Fire)
            {
                multiplier *= 2;
            }
            if (typeEffectivenessMultiplier > 1 && TypeResistantBerryItemMap.TryGetValue(defensivePokemon.HeldItem, out var berryResistantType) && move.Type.Name == berryResistantType)
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

        private static double GetTerrainMultiplier(string terrain, Move move, StattedPokemon offendingPokemon, StattedPokemon defendingPokemon)
        {
            double multiplier = 1;
            if (offendingPokemon.Ability == Abilities.Levitate || offendingPokemon.Types.Any(type => type == Types.Flying))
            {
                if (!(offendingPokemon.Ability == Abilities.QuarkDrive && terrain == Terrain.ElectricTerrain))
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
