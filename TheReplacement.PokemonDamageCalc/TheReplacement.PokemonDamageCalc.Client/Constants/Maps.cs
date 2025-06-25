using PokeApiNet;
using System.Collections.ObjectModel;
using TheReplacement.PokemonDamageCalc.Client.DataModel;
using TheReplacement.PokemonDamageCalc.Client.DTOs;

namespace TheReplacement.PokemonDamageCalc.Client.Constants
{
    public static class Maps
    {
        public static readonly ReadOnlyDictionary<string, Func<StattedPokemon, int>> TrueDamageMap;
        public static readonly ReadOnlyDictionary<string, Func<StattedPokemon, MoveData, bool>> IsStatBoostByAbilityMap;
        public static readonly ReadOnlyDictionary<string, Func<MoveData, bool>> IsStatBoostByChoiceItemMap;
        public static readonly ReadOnlyDictionary<string, double> OtherStatBoostingItemMap;
        public static readonly ReadOnlyDictionary<string, string> TypeResistantBerryItemMap;
        public static readonly ReadOnlyDictionary<string, Func<StattedPokemon, StattedPokemon, MoveData, int>> ConditionalMovePowerMap;
        public static readonly ReadOnlyDictionary<string, TerrainEffectiveness> TerrainEffectivenessChart;
        public static readonly ReadOnlyDictionary<string, TypeEffectiveness> TypeEffectivenessChart;
        public static readonly ReadOnlyDictionary<string, WeatherEffectiveness> WeatherEffectivenessChart;

        static Maps()
        {
            TrueDamageMap = GetTrueDamageMap();
            IsStatBoostByAbilityMap = GetIsStatBoostByAbilityMap();
            IsStatBoostByChoiceItemMap = GetIsStatBoostByChoiceItemMap();
            OtherStatBoostingItemMap = GetOtherStatBoostingItemMap();
            TypeResistantBerryItemMap = GetTypeResistantBerryItemMap();
            ConditionalMovePowerMap = GetConditionalMovePowerMap();
            TerrainEffectivenessChart = GetTerrainEffectivenessChart();
            TypeEffectivenessChart = GetTypeEffectivenessChart();
            WeatherEffectivenessChart = GetWeatherEffectivenessChart();
        }

        private static ReadOnlyDictionary<string, Func<StattedPokemon, int>> GetTrueDamageMap()
        {
            var dictionary = new Dictionary<string, Func<StattedPokemon, int>>
            {
                {Moves.SonicBoom, (offensivePokemon) => 20},
                {Moves.DragonRage, (offensivePokemon) => 40},
                {Moves.SeismicToss, (offensivePokemon) => offensivePokemon.Level },
                {Moves.NightShade, (offensivePokemon) => offensivePokemon.Level },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, Func<StattedPokemon, MoveData, bool>> GetIsStatBoostByAbilityMap()
        {
            var dictionary = new Dictionary<string, Func<StattedPokemon, MoveData, bool>>
            {
                {Abilities.Overgrow, GetIsStatBoostByAbilityFunction(Types.Grass) },
                {Abilities.Blaze, GetIsStatBoostByAbilityFunction(Types.Fire) },
                {Abilities.Torrent, GetIsStatBoostByAbilityFunction(Types.Water) },
                {Abilities.Swarm, GetIsStatBoostByAbilityFunction(Types.Bug) },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, Func<MoveData, bool>> GetIsStatBoostByChoiceItemMap()
        {
            var dictionary = new Dictionary<string, Func<MoveData, bool>>
            {
                {Items.ChoiceBand, GetIsStatBoostByChoiceItemFunction(DamageClasses.Physical) },
                {Items.ChoiceSpecs, GetIsStatBoostByChoiceItemFunction(DamageClasses.Special) },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, double> GetOtherStatBoostingItemMap()
        {
            // todo: rest of stat boosting items
            var dictionary = new Dictionary<string, double>
            {
                {Items.SilkScarf, 1.2 },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, string> GetTypeResistantBerryItemMap()
        {
            var dictionary = new Dictionary<string, string>
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

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, Func<StattedPokemon, StattedPokemon, MoveData, int>> GetConditionalMovePowerMap()
        {
            // todo: https://bulbapedia.bulbagarden.net/wiki/Category:Moves_that_have_variable_power
            var dictionary = new Dictionary<string, Func<StattedPokemon, StattedPokemon, MoveData, int>>
            {
                {Moves.Return, (offensivePokemon, _, _) => Math.Max((int)(offensivePokemon.Friendship/2.5),1) },
                {Moves.Frustation, (offensivePokemon, _, _) => Math.Max((int)((255-offensivePokemon.Friendship)/2.5),1) },
                {Moves.HeavySlam, (offensivePokemon, defensivePokemon, _) => HeavySlamMovePower(offensivePokemon, defensivePokemon) },
                {Moves.HeatCrash, (offensivePokemon, defensivePokemon, _) => HeavySlamMovePower(offensivePokemon, defensivePokemon) },
                {Moves.ElectroBall, (offensivePokemon, defensivePokemon, _) => GetElectroBallMovePower(offensivePokemon, defensivePokemon) },
                {Moves.GyroBall, (offensivePokemon, defensivePokemon, _) => GetGyroBallMovePower(offensivePokemon, defensivePokemon) },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, TerrainEffectiveness> GetTerrainEffectivenessChart()
        {
            var dictionary = new Dictionary<string, TerrainEffectiveness>
            {
                { Terrain.None, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { Terrain.ElectricTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Electric],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = [Moves.Psyblade]
                } },
                { Terrain.GrassyTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Grass],
                    WeakenedOffensiveTypes = [Types.Ground],
                    AdditionalBoostedMoves = []
                } },
                { Terrain.PsychicTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Psychic],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { Terrain.MisttyTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    WeakenedOffensiveTypes = [Types.Dragon],
                    AdditionalBoostedMoves = []
                } },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, TypeEffectiveness> GetTypeEffectivenessChart()
        {
            var dictionary = new Dictionary<string, TypeEffectiveness>
            {
                { Types.Fire, new TypeEffectiveness
                {
                    Weaknesses = [Types.Water, Types.Rock, Types.Ground],
                    Resistances = [Types.Fire, Types.Grass, Types.Bug, Types.Ice, Types.Steel, Types.Fairy],
                    Immunities = []
                } },
                { Types.Water, new TypeEffectiveness
                {
                    Weaknesses = [Types.Grass, Types.Electric],
                    Resistances = [Types.Fire, Types.Water, Types.Ice, Types.Steel],
                    Immunities = []
                } },
                { Types.Grass, new TypeEffectiveness
                {
                    Weaknesses = [Types.Fire, Types.Flying, Types.Bug, Types.Ice, Types.Poison],
                    Resistances = [Types.Grass, Types.Water, Types.Electric, Types.Ground],
                    Immunities = []
                } },
                { Types.Electric, new TypeEffectiveness
                {
                    Weaknesses = [Types.Ground],
                    Resistances = [Types.Electric, Types.Steel],
                    Immunities = []
                } },
                { Types.Normal, new TypeEffectiveness
                {
                    Weaknesses = [Types.Fighting],
                    Resistances = [],
                    Immunities = [Types.Ghost]
                } },
                { Types.Flying, new TypeEffectiveness
                {
                    Weaknesses = [Types.Electric, Types.Rock, Types.Ice],
                    Resistances = [Types.Bug, Types.Fighting],
                    Immunities = [Types.Ground]
                } },
                { Types.Rock, new TypeEffectiveness
                {
                    Weaknesses = [Types.Water, Types.Grass, Types.Ground, Types.Fighting, Types.Steel],
                    Resistances = [Types.Normal, Types.Fire, Types.Poison],
                    Immunities = [Types.Ground]
                } },
                { Types.Ground, new TypeEffectiveness
                {
                    Weaknesses = [Types.Water, Types.Grass, Types.Ice],
                    Resistances = [Types.Rock, Types.Poison],
                    Immunities = [Types.Electric]
                } },
                { Types.Poison, new TypeEffectiveness
                {
                    Weaknesses = [Types.Ground, Types.Psychic],
                    Resistances = [Types.Grass, Types.Bug, Types.Fighting, Types.Fairy],
                    Immunities = []
                } },
                { Types.Bug, new TypeEffectiveness
                {
                    Weaknesses = [Types.Fire, Types.Flying, Types.Rock],
                    Resistances = [Types.Grass, Types.Ground, Types.Fighting],
                    Immunities = []
                } },
                { Types.Fighting, new TypeEffectiveness
                {
                    Weaknesses = [Types.Flying, Types.Psychic, Types.Fairy],
                    Resistances = [Types.Rock, Types.Bug, Types.Dark],
                    Immunities = []
                } },
                { Types.Ghost, new TypeEffectiveness
                {
                    Weaknesses = [Types.Ghost, Types.Dark],
                    Resistances = [Types.Poison, Types.Bug],
                    Immunities = [Types.Normal, Types.Fighting]
                } },
                { Types.Psychic, new TypeEffectiveness
                {
                    Weaknesses = [Types.Ghost, Types.Dark, Types.Bug],
                    Resistances = [Types.Fighting, Types.Psychic],
                    Immunities = []
                } },
                { Types.Ice, new TypeEffectiveness
                {
                    Weaknesses = [Types.Fire, Types.Rock, Types.Fighting, Types.Steel],
                    Resistances = [Types.Ice],
                    Immunities = []
                } },
                { Types.Dragon, new TypeEffectiveness
                {
                    Weaknesses = [Types.Ice, Types.Dragon, Types.Fairy],
                    Resistances = [Types.Grass, Types.Fire, Types.Water, Types.Electric],
                    Immunities = []
                } },
                { Types.Dark, new TypeEffectiveness
                {
                    Weaknesses = [Types.Bug, Types.Fighting, Types.Fairy],
                    Resistances = [Types.Ghost, Types.Dark],
                    Immunities = [Types.Psychic]
                } },
                { Types.Steel, new TypeEffectiveness
                {
                    Weaknesses = [Types.Fire, Types.Ground, Types.Fighting],
                    Resistances = [Types.Grass, Types.Normal, Types.Flying, Types.Rock, Types.Bug, Types.Psychic, Types.Steel, Types.Ice, Types.Dragon, Types.Fairy],
                    Immunities = [Types.Poison]
                } },
                { Types.Fairy, new TypeEffectiveness
                {
                    Weaknesses = [Types.Poison, Types.Steel],
                    Resistances = [Types.Bug, Types.Fighting, Types.Dark],
                    Immunities = [Types.Dragon]
                } },
            };

            return dictionary.AsReadOnly();
        }

        private static ReadOnlyDictionary<string, WeatherEffectiveness> GetWeatherEffectivenessChart()
        {
            var dictionary = new Dictionary<string, WeatherEffectiveness>
            {
                { Weather.None, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    BoostedDefensiveTypes = [],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { Weather.HarshSunlight, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Fire],
                    BoostedDefensiveTypes = [],
                    WeakenedOffensiveTypes = [Types.Water],
                    AdditionalBoostedMoves = [Moves.HydroSteam]
                } },
                { Weather.Rain, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Water],
                    BoostedDefensiveTypes = [],
                    WeakenedOffensiveTypes = [Types.Fire],
                    AdditionalBoostedMoves = []
                } },
                { Weather.Sandstorm, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    BoostedDefensiveTypes = [Types.Rock],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { Weather.Hail, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    BoostedDefensiveTypes = [Types.Ice],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
            };

            return dictionary.AsReadOnly();
        }

        private static Func<StattedPokemon, MoveData, bool> GetIsStatBoostByAbilityFunction(string type)
        {
            return (offensivePokemon, move) => move.Type == type && offensivePokemon.HP >= 2 * offensivePokemon.CurrentHP;
        }

        private static Func<MoveData, bool> GetIsStatBoostByChoiceItemFunction(string damamgeClass)
        {
            return (move) => move.DamageClass == damamgeClass;
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
    }
}
