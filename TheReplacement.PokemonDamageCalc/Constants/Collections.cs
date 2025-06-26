namespace TheReplacement.PokemonDamageCalc.Constants
{
    public static class Collections
    {
        public static readonly ICollection<string>
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
            BulletMoves = [],
            MultiTargetMoves = [MoveTargets.AllOpponents, MoveTargets.AllOtherPokemon],
            DefendedSuperEffective = [Abilities.Filter, Abilities.PrismArmor, Abilities.SolidRock],
            IgnoreProtectMoves = [Moves.DoomDesire, Moves.Feint, Moves.FutureSight, Moves.HyperDrill, Moves.HyperspaceFury, Moves.HyperspaceHole, Moves.HyperspaceHole, Moves.MightyCleave, Moves.PhantomForce, Moves.ShadowForce],
            IgnoreSubstitute = [Moves.HyperspaceFury, Moves.HyperspaceHole, Moves.SpectralThief];
    }
}
