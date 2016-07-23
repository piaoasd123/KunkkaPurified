using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Ensage;
using Ensage.Common.Menu;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;

namespace LastHitIndicator
{
	internal class LastHitIndicator
	{
		private class HealthBar
		{
			public Vector2 Position;
			public Vector2 Size;
			public Color Color;
			public List<AttackMarker> AttackMarkers = null;

			public class AttackMarker
			{
				public Vector2 Position;
				public Vector2 Size;
				public Color Color;
			}

			public void Render()
			{
				Drawing.DrawRect(this.Position, this.Size, this.Color);
			}

			public void RenderOutline()
			{
				Drawing.DrawRect(this.Position, this.Size, Color.Black, true);
			}

			public void RenderMarkers()
			{
				if (AttackMarkers != null)
				{
					foreach (var marker in this.AttackMarkers)
					{
						Drawing.DrawRect(marker.Position, marker.Size, marker.Color);
					}
				}
			}
		}

		private class KillThresholdIndicator
		{
			public Vector2 Position;
			public Vector2 Size;
			public Color Color;
		}
		public static void Init()
		{
			Game.OnUpdate += OnUpdate;
			Drawing.OnDraw += OnDraw;
		}

		public static void OnUpdate(EventArgs args)
		{
			Hero hero = ObjectManager.LocalHero;
			// hero.Hold();
			// hero.Attack(Game.MousePosition);
			
			

	}

		public static void OnDraw(EventArgs args)
		{
			Hero hero = ObjectManager.LocalHero;
			try
			{
				var creeps =
					ObjectManager.GetEntities<Unit>()
						.Where(x => (
									x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
									|| x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
									|| x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
									)
									&& x.IsAlive
									&& x.IsVisible
									);

				var towers = ObjectManager.GetEntities<Unit>().Where(x =>
					(
					x.ClassID == ClassID.CDOTA_BaseNPC_Tower
					//&& x.isNotAlly()
					));

				


				foreach (var creep in creeps)
				{
					// If creep is off screen, don't draw
					Vector2 creepScreenPos;
					var creepPos = creep.Position + new Vector3(0, 0, creep.HealthBarOffset);
					if (!Drawing.WorldToScreen(creepPos, out creepScreenPos)) continue;

					// Find nearest tower to the creep
					Unit nearestOpposingTower = null;
					foreach (var tower in towers)
					{
						if (nearestOpposingTower == null
							|| tower.Distance2D(creep.Position) < nearestOpposingTower.Distance2D(creep.Position)
							)
						{
							if (tower.isNotAllyWithUnit(creep))
							{
								nearestOpposingTower = tower;
							}
						}
					}

					float towerEffectiveDamage = nearestOpposingTower.getEffectiveAttackDamageAgainstUnit(creep).getAvg();
					float predictedHealthAfterTowerAttacks = creep.Health % towerEffectiveDamage;
					float heroEffectiveDamage = hero.getEffectiveAttackDamageAgainstUnit(creep).getAvg();
					float heroEffectiveDamagePercent = heroEffectiveDamage / creep.MaximumHealth;

					float creepHealthPercent = (float)creep.Health / creep.MaximumHealth * 1.00f;
					float creepHealthAfterTowerAttacksPercent = predictedHealthAfterTowerAttacks / creep.MaximumHealth * 1.00f;

					if (creep.isInRangeOfUnit(nearestOpposingTower))
					{
						creepHealthAfterTowerAttacksPercent = predictedHealthAfterTowerAttacks / creep.MaximumHealth * 1.00f;
					}

					// Valve uses non-linear interpolation for health bar display
					var healthBarPos = HUDInfo.GetHPbarPosition(creep) + new Vector2(11, 20.5f);
					float healthBarLength = HUDInfo.GetHPBarSizeX(creep) * .794f;

					// Create health bar and attacker markers
					HealthBar healthBar = new HealthBar();
					healthBar.Position = healthBarPos;
					healthBar.Size = new Vector2(
						LastHitIndicatorUtils.getHealthPercentForDisplay(creepHealthPercent) * healthBarLength,
						HUDInfo.GetHpBarSizeY(creep) - 4);
					healthBar.Color = new Color(180, 205, 205, 40);
					int numAttacks = (int)Math.Truncate(creepHealthPercent / heroEffectiveDamagePercent);
					healthBar.AttackMarkers = new List<HealthBar.AttackMarker>();
					int i = 1;
					for (; i <= numAttacks; i++)
					{
						var newMarker = new HealthBar.AttackMarker();
						newMarker.Position = healthBarPos
							+ new Vector2(healthBarLength, 0) * LastHitIndicatorUtils.getHealthPercentForDisplay(heroEffectiveDamagePercent * i);
						newMarker.Size = new Vector2(2, HUDInfo.GetHpBarSizeY(creep) - 4);
						newMarker.Color = new Color(20, 20, 20, 90);
						healthBar.AttackMarkers.Add(newMarker);
					}

					// Tower Damage Indicator
					HealthBar towerIndicator = new HealthBar();
					towerIndicator.Position = healthBarPos;
					towerIndicator.Size = new Vector2(
						LastHitIndicatorUtils.getHealthPercentForDisplay(creepHealthAfterTowerAttacksPercent) * healthBarLength,
						HUDInfo.GetHpBarSizeY(creep) - 4);
					towerIndicator.Color = new Color(125, 0, 125, 100);

					

					if (creep.isInRangeOfUnit(nearestOpposingTower))
					{
						if (creepHealthAfterTowerAttacksPercent > heroEffectiveDamagePercent * 3)
						{
							// Pretty much out of reach, gray Indicator
							towerIndicator.Color = new Color(30, 30, 30, 100);
						}
						else if (creepHealthAfterTowerAttacksPercent > heroEffectiveDamagePercent * 2
						&& creepHealthAfterTowerAttacksPercent < heroEffectiveDamagePercent * 3)
						{
							// between 2 - 3 hits, yellow indicator
							towerIndicator.Color = new Color(255, 255, 40, 100);
						}
						else if (creepHealthAfterTowerAttacksPercent > heroEffectiveDamagePercent
							&& creepHealthAfterTowerAttacksPercent < heroEffectiveDamagePercent * 2)
						{
							// between 1 - 2 hits, red indicator
							towerIndicator.Color = new Color(255, 0, 0, 100);
						}
						else if (creepHealthAfterTowerAttacksPercent < heroEffectiveDamagePercent)
						{
							// less than 1 hit, green indicator
							towerIndicator.Color = new Color(127, 255, 0, 100);
						}
					}

					if (!creep.isInRangeOfUnit(nearestOpposingTower))
					{
						if (creepHealthAfterTowerAttacksPercent > heroEffectiveDamagePercent * 3)
						{
							// Pretty much out of reach, gray Indicator
							//towerIndicator.Color = new Color(30, 30, 30, 100);
						}

						if (creepHealthPercent > heroEffectiveDamagePercent * 2
						&& creepHealthPercent < heroEffectiveDamagePercent * 3)
						{
							// between 2 - 3 hits, yellow indicator
							healthBar.Color = new Color(255, 255, 40, 100);
						}
						else if (creepHealthPercent > heroEffectiveDamagePercent
							&& creepHealthPercent < heroEffectiveDamagePercent * 2)
						{
							// between 1 - 2 hits, red indicator
							healthBar.Color = new Color(255, 0, 0, 100);
						}
						else if (creepHealthPercent < heroEffectiveDamagePercent)
						{
							// less than 1 hit, green indicator
							healthBar.Color = new Color(127, 255, 0, 100);
						}
					}

					// Draw health bar and attack markers
					healthBar.Render();
					if (creep.isInRangeOfUnit(nearestOpposingTower))
					{
						towerIndicator.Render();
					}
					healthBar.RenderOutline();
					healthBar.RenderMarkers();

				}
			}
			catch (Exception)
			{
				//
			}
		}
	}
}
