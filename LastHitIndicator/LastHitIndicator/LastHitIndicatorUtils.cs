using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Menu;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using SharpDX.Direct3D9;

namespace LastHitIndicator
{
	public class Damage
	{
		public Damage(DamageType type, float min, float max)
		{
			this.type = type;
			this.min = min;
			this.max = max;
			this.avg = (min + max) / 2;
		}

		public static Damage operator + (Damage a, float b)
		{
			a.min += b;
			a.max += b;
			a.avg += b;
			return a;
		}

		public DamageType getDamageType()
		{
			return type;
		}

		public float getMin()
		{
			return min;
		}

		public float getMax()
		{
			return max;
		}

		public float getAvg()
		{
			return avg;
		}

		private DamageType type;
		private float min = 0;
		private float max = 0;
		private float avg = 0;
	}

	public static class LastHitIndicatorUtils
	{

		public static bool hasQuellingBlade(this Hero hero)
		{
			return hero.FindItem("item_quelling_blade") != null;
		}

		public static bool isAlly(this Unit unit)
		{
			return unit.Team == ObjectManager.LocalHero.Team;
		}

		public static bool isNotAllyWithHero(this Unit unit)
		{
			return unit.Team != ObjectManager.LocalHero.Team;
		}

		public static bool isNotAllyWithUnit(this Unit a, Unit b)
		{
			return a.Team != b.Team;
		}

		public static Damage getEffectiveDamageTaken(this Unit unit, Damage inc)
		{
			float resistance = 0;
			if (inc.getDamageType() == DamageType.Physical)
			{
				resistance = unit.DamageResist;
			}
			else if (inc.getDamageType() == DamageType.Magical)
			{
				resistance = unit.MagicDamageResist;
			}
			float newMin = inc.getMin() * (1 - resistance);
			float newMax = inc.getMax() * (1 - resistance);
			Damage result = new Damage(inc.getDamageType(), newMin, newMax);

			return result;
		}

		public static Damage getEffectiveAttackDamageAgainstUnit(this Unit instigator, Unit target)
		{
			float minDamage = 0;
			float maxDamage = 0;
			float bonusDamage = 0;
			if (instigator.GetType() == typeof(Hero))
			{
				Hero hero = (Hero)instigator;

				Ability q, w, e, r;
				q = hero.Spellbook.Spell1;
				w = hero.Spellbook.Spell2;
				e = hero.Spellbook.Spell3;
				r = hero.Spellbook.Spell4;

				switch (hero.ClassID)
				{
					case ClassID.CDOTA_Unit_Hero_Kunkka:
					if (instigator.isNotAllyWithUnit(target))
					{
						if (w.Level > 0 && w.AbilityState == AbilityState.Ready && w.IsAutoCastEnabled)
						{
							bonusDamage = (w.Level - 1) * 15 + 25;
						}
					}
					break;
				}

				if (hero.hasQuellingBlade() && target.isNotAllyWithHero())
				{
					if (hero.IsRanged)
					{
						minDamage = hero.MinimumDamage * 1.15f + hero.BonusDamage;
						maxDamage = hero.MaximumDamage * 1.15f + hero.BonusDamage;
					}
					else
					{
						minDamage = hero.MinimumDamage * 1.4f + hero.BonusDamage;
						maxDamage = hero.MaximumDamage * 1.4f + hero.BonusDamage;
					}
				}
				else
				{
					// hero without quelling blade
					minDamage = instigator.MinimumDamage + hero.BonusDamage;
					maxDamage = instigator.MaximumDamage + hero.BonusDamage;
				}
			}
			else
			{
				// units other than hero
				minDamage = instigator.MinimumDamage + instigator.BonusDamage;
				maxDamage = instigator.MaximumDamage + instigator.BonusDamage;
			}


			Damage rawDamage = new Damage(DamageType.Physical, minDamage, maxDamage);
			rawDamage += bonusDamage;
			Damage damageTaken = getEffectiveDamageTaken(target, rawDamage);
			return damageTaken;
		}

		public static float getHealthPercentForDisplay(float healthPercent)
		{
			return Math.Min(1.0f, healthPercent + (1 - healthPercent) * (1 - healthPercent) * 0.025f);
		}

		public static bool isInRangeOfUnit(this Unit a, Unit b)
		{
			return b.AttackRange > b.Distance2D(a.Position);

		}
	}
}
