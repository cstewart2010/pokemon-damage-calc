namespace TheReplacement.PokemonDamageCalc.Client.DataModel
{
    public class Conditionals
    {
        public bool IsCriticalHit { get; set; }
        public bool AfterGlaiveRush { get; set; }
        public bool LastMoveMissed { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsLaserFocus { get; set; }
        public bool UsedDig { get; set; }
        public bool UsedDive { get; set; }
        public bool UsedFly { get; set; }
        public bool UsedReflect { get; set; }
        public bool UsedLightScreen { get; set; }
        public bool UsedAuroraVeil { get; set; }
        public bool IsFriendGuarded { get; set; }
        public bool IsTagBattle { get; set; }
        public bool ProtectActive { get; set; }
        public bool SubstituteActive { get; set; }
        public bool IsOffensivePokemonTera { get; set; }
        public bool IsDefensivePokemonTera { get; set; }
    }
}
