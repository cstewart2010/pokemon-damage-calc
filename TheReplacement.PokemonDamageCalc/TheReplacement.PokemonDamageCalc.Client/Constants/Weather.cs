using System.Collections.ObjectModel;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Constants
{
    public static class Weather
    {
        public const string None = "No Weather";
        public const string HarshSunlight = "Harsh Sunlight";
        public const string Rain = "Rain";
        public const string Sandstorm = "Sandstorm";
        public const string Hail = "Hail";

        public static readonly ReadOnlyDictionary<string, WeatherEffectiveness> EffectivenessChart;

        static Weather()
        {
            var dictionary = new Dictionary<string, WeatherEffectiveness>
            {
                { None, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    BoostedDefensiveTypes = [],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { HarshSunlight, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Fire],
                    BoostedDefensiveTypes = [],
                    WeakenedOffensiveTypes = [Types.Water],
                    AdditionalBoostedMoves = [Moves.HydroSteam]
                } },
                { Rain, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Water],
                    BoostedDefensiveTypes = [],
                    WeakenedOffensiveTypes = [Types.Fire],
                    AdditionalBoostedMoves = []
                } },
                { Sandstorm, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    BoostedDefensiveTypes = [Types.Rock],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { Hail, new WeatherEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    BoostedDefensiveTypes = [Types.Ice],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
            };

            EffectivenessChart = dictionary.AsReadOnly();
        }
    }
}
