﻿//*
using Ensage;

namespace Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using global::Heroes.Abilities;

    using SharpDX;

    internal class Abaddon : IHero
    {
        #region Fields

        private readonly List<IAbility> allSpells = new List<IAbility>();

        private AbaddonMenu menuManager;
        private Sleeper sleeper;

        // Hero
        private Hero hero;
        private Team heroTeam;
        private Unit target;

        // Heroes Abilities
        private MistCoil coil;
        // Misc
        private ParticleEffect targetParticle;
        #endregion

        public void OnClose()
        {
            menuManager.OnClose();
            allSpells.Clear();
            targetParticle?.Dispose();
            targetParticle = null;
            target = null;
        }

        public void OnDraw()
        {
            if (!menuManager.IsEnabled)
            {
                return;
            }

            target = TargetSelector.GetLowestHPCreep(hero, 600);
            if (target == null || coil.CastRange < hero.Distance2D(target) || !hero.IsAlive
                || target.IsLinkensProtected() || target.IsMagicImmune() || target.Health > coil.Damage)
            {
                if (targetParticle != null)
                {
                    targetParticle.Dispose();
                    targetParticle = null;
                }
                target = null;
                return;
            }

            if (targetParticle == null)
            {
                targetParticle = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
            }

            targetParticle.SetControlPoint(2, hero.Position);
            targetParticle.SetControlPoint(6, new Vector3(1, 0, 0));
            targetParticle.SetControlPoint(7, target.Position);
            /*
            

            

            
            */
        }

        public void OnExecuteAbilitiy(Player sender, ExecuteOrderEventArgs args)
        {

            if (!sender.Equals(hero.Player) || !menuManager.IsEnabled)
            {
                return;
            }
            /*
            manualTarget = null;

            var order = args.Order;

            if (xMark.PhaseStarted && (order == Order.Hold || order == Order.Stop))
            {
                xMark.PhaseStarted = false;
                targetLocked = false;
                return;
            }

            var ability = args.Ability;

            if (ability == null)
            {
                return;
            }

            if (ability.Equals(xMark.Ability) && order == Order.AbilityTarget)
            {
                var newTarget = (Hero)args.Target;
                if (newTarget.Team != heroTeam)
                {
                    manualTarget = newTarget;
                }
            }
            else if (ability.Equals(ghostShip.Ability) && order == Order.AbilityLocation)
            {
                ghostShip.Position = hero.Position.Extend(args.TargetPosition, ghostShip.CastRange);
            }
            */
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;
            sleeper = new Sleeper();
            menuManager = new AbaddonMenu(hero.Name);
            allSpells.Add(coil = new MistCoil(hero.Spellbook.SpellQ));
        }

        public void OnUpdate()
        {
            /*
            // Housekeeping...
            {
                if (sleeper.Sleeping)
                {
                    return;
                }

                if (Game.IsPaused || !hero.IsAlive || !hero.CanCast() || hero.IsChanneling() || !menuManager.IsEnabled)
                {
                    sleeper.Sleep(333);
                    return;
                }
            }

            if (!xMark.PositionUpdated && targetLocked)
            {
                if (xMark.TimeCasted + xMark.CastPoint >= Game.RawGameTime && target != null && target.IsVisible)
                {
                    xMark.Position = target.NetworkPosition;
                    return;
                }
                xMark.PhaseStarted = false;
                xMark.PositionUpdated = true;
            }

            if (ghostShip.IsInPhase)
            {
                ghostShip.HitTime = Game.RawGameTime
                                    + ghostShip.CastRange
                                    / (hero.AghanimState() ? ghostShip.AghanimSpeed : ghostShip.Speed);
            }

            if (manualTarget != null && xMark.IsInPhase && !xMark.PhaseStarted)
            {
                xMark.PhaseStarted = true;
                target = manualTarget;
                targetLocked = true;
                xMark.Position = target.NetworkPosition;
                xMark.TimeCasted = Game.RawGameTime + Game.Ping / 1000;
                manualTarget = null;
                return;
            }

            TPHandler();

            if (menuManager.HitAndRunEnabled)
            {
                var blink = hero.FindItem("item_blink");
                if (blink == null)
                {
                    return;
                }

                var armlet = hero.FindItem("item_armlet");
                var armletEnabled = hero.HasModifier("modifier_item_armlet_unholy_strength");

                var hitTarget =
                    (Unit)
                    Creeps.All.OrderBy(x => x.Distance2D(Game.MousePosition))
                        .FirstOrDefault(
                            x =>
                            x.Team != heroTeam && x.IsSpawned && x.IsVisible && x.Distance2D(hero) < 2000
                            && x.Distance2D(Game.MousePosition) < 250)
                    ?? Heroes.All.OrderBy(x => x.Distance2D(Game.MousePosition))
                           .FirstOrDefault(
                               x =>
                               x.Team != heroTeam && x.IsVisible && x.Distance2D(hero) < 2000
                               && x.Distance2D(Game.MousePosition) < 250);

                if (xReturn.CanBeCasted && !blink.CanBeCasted()
                    && (hitTarget == null || !hitTarget.IsAlive || tideBringer.Casted))
                {
                    if (armlet != null && armletEnabled)
                    {
                        armlet.ToggleAbility();
                    }
                    xReturn.UseAbility();
                    sleeper.Sleep(2000);
                    return;
                }

                if (hitTarget == null || !hitTarget.IsAlive)
                {
                    return;
                }

                if (armlet != null && !armletEnabled)
                {
                    armlet.ToggleAbility();
                }

                if (hitTarget.Distance2D(hero) > 1200)
                {
                    hero.Move(hitTarget.Position);
                    sleeper.Sleep(500);
                    return;
                }

                if (xMark.CanBeCasted)
                {
                    xMark.UseAbility(hero);
                    sleeper.Sleep(xMark.GetSleepTime);
                    return;
                }

                if (blink.CanBeCasted() && hero.HasModifier("modifier_kunkka_x_marks_the_spot"))
                {
                    blink.UseAbility(hitTarget.Position.Extend(hero.Position, hero.AttackRange));
                    tideBringer.UseAbility(hitTarget, true);
                    sleeper.Sleep(300);
                    return;
                }
            }

            if (menuManager.TorrentOnRuneEnabled)
            {
                var gameTime = Game.GameTime;

                if (gameTime % 120 < (gameTime > 0 ? 119.5 : -0.5) - torrent.AdditionalDelay - Game.Ping / 1000
                    || !torrent.CanBeCasted)
                {
                    return;
                }

                var rune = runePositions.OrderBy(x => x.Distance2D(hero)).First();

                if (rune.Distance2D(hero) > torrent.CastRange)
                {
                    return;
                }

                torrent.UseAbility(rune);
                sleeper.Sleep(torrent.GetSleepTime);
                return;
            }

            if (menuManager.ComboEnabled)
            {
                var fullCombo = menuManager.FullComboEnabled;

                if (!comboStarted)
                {
                    if (target == null)
                    {
                        return;
                    }

                    if (!CheckCombo(fullCombo, targetLocked))
                    {
                        return;
                    }

                    var manaRequired = allSpells.Where(x => (x != ghostShip || fullCombo) && x.CanBeCasted)
                        .Aggregate(0u, (current, spell) => current + spell.ManaCost);

                    if (manaRequired > hero.Mana)
                    {
                        return;
                    }

                    if (!targetLocked)
                    {
                        xMark.Position = target.NetworkPosition;
                    }

                    targetLocked = true;
                    comboStarted = true;
                }

                if (target == null || !target.IsValid || target.IsMagicImmune())
                {
                    return;
                }

                if (xMark.CanBeCasted)
                {
                    xMark.UseAbility(target);
                    return;
                }

                if (ghostShip.CanBeCasted && fullCombo)
                {
                    if (!hero.AghanimState() && torrent.CanBeCasted)
                    {
                        ghostShip.UseAbility(xMark.Position);
                        sleeper.Sleep(ghostShip.GetSleepTime);
                        return;
                    }

                    if (torrent.Casted
                        && Game.RawGameTime
                        >= torrent.HitTime - ghostShip.CastPoint - xReturn.CastPoint - Game.Ping / 1000)
                    {
                        ghostShip.UseAbility(GetTorrentThinker()?.Position ?? xMark.Position);
                    }
                }

                if (torrent.CanBeCasted
                    && (!fullCombo || (ghostShip.CanBeCasted || !hero.AghanimState() && ghostShip.Cooldown > 2)))
                {
                    torrent.UseAbility(xMark.Position);
                    sleeper.Sleep(torrent.GetSleepTime);
                    return;
                }

                if (xReturn.CanBeCasted && torrent.Casted
                    && Game.RawGameTime >= torrent.HitTime - xReturn.CastPoint - Game.Ping / 1000)
                {
                    xReturn.UseAbility();
                    sleeper.Sleep(xReturn.GetSleepTime);
                    return;
                }
            }
            else if (comboStarted && !xMark.IsInPhase && xReturn.Casted)
            {
                comboStarted = false;
                targetLocked = false;
            }

            if (xMark.Casted && xReturn.CanBeCasted && menuManager.AutoReturnEnabled && !comboStarted)
            {
                var gameTime = Game.RawGameTime;

                var pudge =
                    Heroes.GetByTeam(heroTeam)
                        .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && x.IsAlive && !x.IsIllusion);

                if (pudge != null)
                {
                    var hook = pudge.Spellbook.SpellQ;

                    if (hook.IsInAbilityPhase)
                    {
                        if (hookCasted)
                        {
                            return;
                        }

                        hookHitTime = CalculateHitTime(pudge, hook, gameTime, 0);

                        if (hookHitTime > 0)
                        {
                            hookCasted = true;
                        }
                    }
                    else if (hookCasted && hook.AbilityState != AbilityState.OnCooldown)
                    {
                        hookCasted = false;
                    }
                }

                var mirana =
                    Heroes.GetByTeam(heroTeam)
                        .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Mirana && x.IsAlive && !x.IsIllusion);

                if (mirana != null)
                {
                    var arrow = mirana.Spellbook.SpellW;

                    if (arrow.IsInAbilityPhase)
                    {
                        if (arrowCasted)
                        {
                            return;
                        }

                        arrowHitTime = CalculateHitTime(mirana, arrow, gameTime);

                        if (arrowHitTime > 0)
                        {
                            arrowCasted = true;
                        }
                    }
                    else if (arrowCasted && arrow.AbilityState != AbilityState.OnCooldown)
                    {
                        arrowCasted = false;
                    }
                }

                var delay = xReturn.CastPoint + Game.Ping / 1000;

                if (torrent.Casted)
                {
                    var torrentThinker = GetTorrentThinker();

                    if (torrentThinker != null)
                    {
                        if (xMark.Position.Distance2D(torrentThinker) > torrent.Radius)
                        {
                            return;
                        }

                        var modifier = torrentThinker.FindModifier("modifier_kunkka_torrent_thinker");
                        var hitTime = torrent.AdditionalDelay - modifier.ElapsedTime - 0.15;

                        if (hitTime <= delay)
                        {
                            xReturn.UseAbility();
                            targetLocked = false;
                        }
                    }
                }

                if (ghostShip.JustCasted)
                {
                    var hitTime = ghostShip.HitTime;
                    if (!hero.AghanimState())
                    {
                        hitTime += 0.25 - 0.25 / (150 / Math.Min(Game.Ping, 150));
                    }
                    else
                    {
                        hitTime -= 0.25 / (150 / Math.Min(Game.Ping, 150));
                    }
                    if (xMark.Position.Distance2D(ghostShip.Position) <= ghostShip.Radius && hitTime <= gameTime - delay)
                    {
                        xReturn.UseAbility();
                    }
                }

                if (arrowCasted && gameTime >= arrowHitTime - delay)
                {
                    xReturn.UseAbility();
                    targetLocked = false;
                    arrowCasted = false;
                }

                if (hookCasted && gameTime >= hookHitTime - delay)
                {
                    xReturn.UseAbility();
                    targetLocked = false;
                    hookCasted = false;
                }
            }

            if (targetLocked && xMark.Casted && xReturn.Casted)
            {
                targetLocked = false;
            }

            sleeper.Sleep(50);
            */
        }
    }
}
//*/

public interface IHero
{
    void OnClose();

    void OnDraw();

    void OnExecuteAbilitiy(Player sender, ExecuteOrderEventArgs args);

    void OnLoad();

    void OnUpdate();
}