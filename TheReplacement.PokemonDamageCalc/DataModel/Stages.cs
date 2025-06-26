namespace TheReplacement.PokemonDamageCalc.DataModel
{
    public class Stages
    {
        private const int Maximum = 6;
        private const int Minimum = -6;
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
                    _hp = value;
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
                    _attack = value;
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
                    _defense = value;
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
                    _specialAttack = value;
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
                    _specialDefense = value;
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
                    _speed = value;
                }
            }
        }
    }
}
