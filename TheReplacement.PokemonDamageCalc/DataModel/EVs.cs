namespace TheReplacement.PokemonDamageCalc.DataModel
{
    public class EVs
    {
        private const int Total = 510;
        private const int Maximum = 252;
        private const int Minimum = 0;
        private int _hp;
        private int _attack;
        private int _defense;
        private int _specialAttack;
        private int _specialDefense;
        private int _speed;
        public int HP
        {
            get => _hp;
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value + Attack + Defense + SpecialAttack + SpecialDefense + Speed <= Total)
                    {
                        _hp = value;
                    }
                }
            }
        }
        public int Attack
        {
            get => _attack;
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value + HP + Defense + SpecialAttack + SpecialDefense + Speed <= Total)
                    {
                        _attack = value;
                    }
                }
            }
        }
        public int Defense
        {
            get => _defense;
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value + HP + Attack + SpecialAttack + SpecialDefense + Speed <= Total)
                    {
                        _defense = value;
                    }
                }
            }
        }
        public int SpecialAttack
        {
            get => _specialAttack;
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value + HP + Attack + Defense + SpecialDefense + Speed <= Total)
                    {
                        _specialAttack = value;
                    }
                }
            }
        }
        public int SpecialDefense
        {
            get => _specialDefense;
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value + HP + Attack + Defense + SpecialAttack + Speed <= Total)
                    {
                        _specialDefense = value;
                    }
                }
            }
        }
        public int Speed
        {
            get => _speed;
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value + HP + Attack + Defense + SpecialAttack + SpecialDefense <= Total)
                    {
                        _speed = value;
                    }
                }
            }
        }
    }
}
