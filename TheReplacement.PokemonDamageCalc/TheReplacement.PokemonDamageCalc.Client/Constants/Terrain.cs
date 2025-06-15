using System.Collections.ObjectModel;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Constants
{
    public static class Terrain
    {
        public const string None = "No Terrain";
        public const string ElectricTerrain = "Electric Terrain";
        public const string GrassyTerrain = "Grassy Terrain";
        public const string PsychicTerrain = "Psychic Terrain";
        public const string MisttyTerrain = "Misty Terrain";

        public static readonly ReadOnlyDictionary<string, TerrainEffectiveness> EffectivenessChart;

        static Terrain()
        {
            var dictionary = new Dictionary<string, TerrainEffectiveness>
            {
                { None, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { ElectricTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Electric],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = [MoveNames.Psyblade]
                } },
                { GrassyTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Grass],
                    WeakenedOffensiveTypes = [Types.Ground],
                    AdditionalBoostedMoves = []
                } },
                { PsychicTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [Types.Psychic],
                    WeakenedOffensiveTypes = [],
                    AdditionalBoostedMoves = []
                } },
                { MisttyTerrain, new TerrainEffectiveness
                {
                    BoostedOffensiveTypes = [],
                    WeakenedOffensiveTypes = [Types.Dragon],
                    AdditionalBoostedMoves = []
                } },
            };
            EffectivenessChart = dictionary.AsReadOnly();
        }
    }
}
