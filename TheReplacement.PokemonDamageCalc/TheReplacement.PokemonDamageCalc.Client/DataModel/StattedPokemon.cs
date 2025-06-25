namespace TheReplacement.PokemonDamageCalc.Client.DataModel
{
    using TheReplacement.PokemonDamageCalc.Client.Constants;
    using TheReplacement.PokemonDamageCalc.Client.DTOs;

    public class StattedPokemon
    {
        private RawPokemon _pokemon;
        private NatureData _nature;
        private double _currentHp;

        public StattedPokemon(RawPokemon pokemon, NatureData nature)
        {
            _pokemon = pokemon;
            _nature = nature;
            Stages = new Stages();
            IVs = new IVs();
            EVs = new EVs();
        }

        public int Level { get; set; }
        public int Friendship { get; set; }
        public string Ability { get; set; } = string.Empty;
        public string HeldItem { get; set; } = string.Empty;
        public double CurrentHP
        {
            get => _currentHp;
            set
            {
                if (value > HP)
                {
                    _currentHp = HP;
                }
                else if (value < 0)
                {
                    _currentHp = 0;
                }
                else
                {
                    _currentHp = value;
                }
            }
        }
        public string? TeraType { get; set; }
        public Stages Stages { get; }
        public IVs IVs { get; }
        public EVs EVs { get; }
        public string Species => _pokemon.SpeciesName;
        public string Nature => _nature.Name;
        public double HP => GetHp();
        public double Attack => GetStat(Stats.Attack, _pokemon.Stats.Attack, IVs.Attack, EVs.Attack, Stages.Attack);
        public double Defense => GetStat(Stats.Defense, _pokemon.Stats.Defense, IVs.Defense, EVs.Defense, Stages.Defense);
        public double SpecialAttack => GetStat(Stats.SpecialAttack, _pokemon.Stats.SpecialAttack, IVs.SpecialAttack, EVs.SpecialAttack, Stages.SpecialAttack);
        public double SpecialDefense => GetStat(Stats.SpecialDefense, _pokemon.Stats.SpecialDefense, IVs.SpecialDefense, EVs.SpecialDefense, Stages.SpecialDefense);
        public double Speed => GetStat(Stats.Speed, _pokemon.Stats.Speed, IVs.Speed, EVs.Speed, Stages.Speed);
        public int Weight => _pokemon.Weight;
        public IEnumerable<string> Types => _pokemon.Types;
        public bool IsAtFullHealth => HP == CurrentHP;

        public void UpdateForm(RawPokemon pokemon)
        {
            _pokemon = pokemon;
        }

        public void UpdateNature(NatureData nature)
        {
            _nature = nature;
        }

        private double GetHp()
        {
            var baseHp = _pokemon.Stats.HP;
            int ivs = IVs.HP;
            double evs = EVs.HP;
            return Math.Floor((2 * baseHp + ivs + Math.Floor(evs / 4)) * Level / 100) + Level + 10;
        }

        private double GetStat(string statName, int baseStat, int ivs, double evs, int stage)
        {
            try
            {
                double natureMultipler = 1;
                if (_nature.DecreasedStat == statName)
                {
                    natureMultipler = 0.9;
                }
                else if (_nature.IncreasedStat == statName)
                {
                    natureMultipler = 1.1;
                }
                var stageMultiplier = stage > 0 ? (2.0 + stage) / 2 : 2.0 / (2 - stage);

                return Math.Floor((Math.Floor((2 * baseStat + ivs + Math.Floor(evs / 4)) * Level / 100) + 5) * natureMultipler * stageMultiplier);
            }
            catch
            {
                return 0;
            }
        }
    }
}
