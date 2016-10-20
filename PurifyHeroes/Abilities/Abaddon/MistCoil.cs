namespace Heroes.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class MistCoil : IAbility
    {
        #region Constructors and Destructors

        public MistCoil(Ability ability)
        {
            Ability = ability;
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public float CastPoint { get; } = 0;
        public float CastRange => Ability.Level > 0 ? Ability.GetCastRange() + 100 : 0;
        public int Damage => Ability.Level > 0 ? Ability.GetDamage(Ability.Level) : 0;
        public uint ManaCost { get; } = 0;

        #endregion

        #region Public Methods and Operators

        public void UseAbility(Unit target, bool queue)
        {
            Ability.UseAbility(target, queue);
        }

        #endregion
    }
}