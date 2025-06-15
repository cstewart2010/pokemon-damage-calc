using System.Collections.ObjectModel;
using TheReplacement.PokemonDamageCalc.Client.DataModel;

namespace TheReplacement.PokemonDamageCalc.Client.Constants
{
    public class Types
    {
        public const string Fire = "fire";
        public const string Water = "water";
        public const string Grass = "grass";
        public const string Electric = "electric";
        public const string Normal = "normal";
        public const string Flying = "flying";
        public const string Rock = "rock";
        public const string Ground = "ground";
        public const string Poison = "poison";
        public const string Bug = "bug";
        public const string Fighting = "fighting";
        public const string Ghost = "ghost";
        public const string Psychic = "psychic";
        public const string Ice = "ice";
        public const string Dragon = "dragon";
        public const string Dark = "dark";
        public const string Steel = "steel";
        public const string Fairy = "fairy";
        public static readonly ReadOnlyDictionary<string, TypeEffectiveness> EffectivenessChart;

        static Types()
        {
            var dictionary = new Dictionary<string, TypeEffectiveness>
            {
                { Fire, new TypeEffectiveness
                {
                    Weaknesses = [Water, Rock, Ground],
                    Resistances = [Fire, Grass, Bug, Ice, Steel, Fairy],
                    Immunities = []
                } },
                { Water, new TypeEffectiveness
                {
                    Weaknesses = [Grass, Electric],
                    Resistances = [Fire, Water, Ice, Steel],
                    Immunities = []
                } },
                { Grass, new TypeEffectiveness
                {
                    Weaknesses = [Fire, Flying, Bug, Ice, Poison],
                    Resistances = [Grass, Water, Electric, Ground],
                    Immunities = []
                } },
                { Electric, new TypeEffectiveness
                {
                    Weaknesses = [Ground],
                    Resistances = [Electric, Steel],
                    Immunities = []
                } },
                { Normal, new TypeEffectiveness
                {
                    Weaknesses = [Fighting],
                    Resistances = [],
                    Immunities = [Ghost]
                } },
                { Flying, new TypeEffectiveness
                {
                    Weaknesses = [Electric, Rock, Ice],
                    Resistances = [Bug, Fighting],
                    Immunities = [Ground]
                } },
                { Rock, new TypeEffectiveness
                {
                    Weaknesses = [Water, Grass, Ground, Fighting, Steel],
                    Resistances = [Normal, Fire, Poison],
                    Immunities = [Ground]
                } },
                { Ground, new TypeEffectiveness
                {
                    Weaknesses = [Water, Grass, Ice],
                    Resistances = [Rock, Poison],
                    Immunities = [Electric]
                } },
                { Poison, new TypeEffectiveness
                {
                    Weaknesses = [Ground, Psychic],
                    Resistances = [Grass, Bug, Fighting, Fairy],
                    Immunities = []
                } },
                { Bug, new TypeEffectiveness
                {
                    Weaknesses = [Fire, Flying, Rock],
                    Resistances = [Grass, Ground, Fighting],
                    Immunities = []
                } },
                { Fighting, new TypeEffectiveness
                {
                    Weaknesses = [Flying, Psychic, Fairy],
                    Resistances = [Rock, Bug, Dark],
                    Immunities = []
                } },
                { Ghost, new TypeEffectiveness
                {
                    Weaknesses = [Ghost, Dark],
                    Resistances = [Poison, Bug],
                    Immunities = [Normal, Fighting]
                } },
                { Psychic, new TypeEffectiveness
                {
                    Weaknesses = [Ghost, Dark, Bug],
                    Resistances = [Fighting, Psychic],
                    Immunities = []
                } },
                { Ice, new TypeEffectiveness
                {
                    Weaknesses = [Fire, Rock, Fighting, Steel],
                    Resistances = [Ice],
                    Immunities = []
                } },
                { Dragon, new TypeEffectiveness
                {
                    Weaknesses = [Ice, Dragon, Fairy],
                    Resistances = [Grass, Fire, Water, Electric],
                    Immunities = []
                } },
                { Dark, new TypeEffectiveness
                {
                    Weaknesses = [Bug, Fighting, Fairy],
                    Resistances = [Ghost, Dark],
                    Immunities = [Psychic]
                } },
                { Steel, new TypeEffectiveness
                {
                    Weaknesses = [Fire, Ground, Fighting],
                    Resistances = [Grass, Normal, Flying, Rock, Bug, Psychic, Steel, Ice, Dragon, Fairy],
                    Immunities = [Poison]
                } },
                { Fairy, new TypeEffectiveness
                {
                    Weaknesses = [Poison, Steel],
                    Resistances = [Bug, Fighting, Dark],
                    Immunities = [Dragon]
                } },
            };

            EffectivenessChart = dictionary.AsReadOnly();
        }
    }
}
