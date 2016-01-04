﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Trookali
{
    class Program
    {
        public const string ChampionName = "Akali";

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static Orbwalking.Orbwalker Orbwalker;

        //Menu
        public static Menu Menu;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;
        private static Items.Item cutlass;
        private static Items.Item botrk;
        private static Items.Item hextech;
        private static Obj_AI_Base target;
        private static bool IsRUse => Player.HasBuff("AkaliShadowDance");

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Akali") return;

            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 290f);
            R = new Spell(SpellSlot.R, 700f);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //SpellMenu
            Menu spellMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            spellMenu.AddItem(new MenuItem("RSelected", "Only R Selected Target").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("harassQ", "Use Q to harass").SetValue(true));
            //LaneClear Menu
            Menu.AddSubMenu(new Menu("Laneclear", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("laneE", "use E to Laneclear").SetValue(true));
            Menu.SubMenu("laneclear")
                .AddItem(new MenuItem("Laneclear Energy", " % Energy").SetValue(new Slider(10, 50, 0)));
            //Drawings
            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Draw_Disabled", "Disable all Drawings").SetValue(false));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));
            //KS
            Menu.AddSubMenu(new Menu("Ks Menu", "KS Menu"));
            Menu.SubMenu("KS Menu").AddItem(new MenuItem("useRks", "use R to Ks").SetValue(true));
            //Credits
            Menu.AddItem(new MenuItem("Credits", "Assembly created by trooperhdx"));
            //Jungleclear
            var jungle = new Menu("JungleClear", "JungleClear");
            Menu.AddSubMenu(jungle);
            jungle.AddItem(new MenuItem("jungleclearE", "Use E to JungleClear").SetValue(true));





            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            hextech = new Items.Item(3146, 700);

            Menu.AddToMainMenu();
            OnDoCast();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
            Game.PrintChat(
                "<font color='#00CC83'>trooperhdx:</font> <font color='#B6250B'>" + Player.ChampionName
                + " Loaded<font color='#00B4D2'> Dont forget to Upvote this Assembly on the Assembly Database! </font>");
        }


        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("Draw_Disabled").GetValue<bool>()) return;

            if (Menu.Item("Qdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.CadetBlue, 3);
            if (Menu.Item("Wdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.IndianRed, 3);
            if (Menu.Item("Edraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.DarkSeaGreen, 3);
            if (Menu.Item("Rdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.BurlyWood, 3);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Menu.Item("useRks").GetValue<bool>())
            {
                Killsecure();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }
        }


        private static void Killsecure()
        {
            var useR = (Menu.Item("useRks").GetValue<bool>());
            var y = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (y != null && y.Health < R.GetDamage(y) && useR)
            {
                R.CastOnUnit(y);
            }
        }

        private static void Combo()
        {
            var x = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var useE = (Menu.Item("useE").GetValue<bool>());
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var useR = (Menu.Item("useR").GetValue<bool>());



            //Itemusage
            if (x != null && Player.Distance(x) <= botrk.Range)
            {
                botrk.Cast(x);
            }
            if (x != null && Player.Distance(x) <= cutlass.Range)
            {
                cutlass.Cast(x);
            }
            if (x != null && Player.Distance(x) <= hextech.Range)
            {
                hextech.Cast(x);
            }

            //combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.Item("useQ").GetValue<bool>() && Q.IsReady()) ;
                {
                    Q.CastOnBestTarget();
                }
                if (Menu.Item("RSelected").GetValue<bool>() && Menu.Item("useR").GetValue<bool>() && R.IsReady()
                    && IsRUse)
                {
                    var k = TargetSelector.GetSelectedTarget();
                    {
                        if (R.IsReady() && k.IsValidTarget() && (Player.Distance(k.Position) < 700))
                        {
                            R.CastOnUnit(k);
                        }
                    }
                    if (Menu.Item("useR").GetValue<bool>() && R.IsReady() && Menu.Item("RSelected").GetValue<bool>() && R.IsReady() && IsRUse)
                    {
                        R.Cast(k);
                    }
                }
            }
        }

        private static void Harass()
        {
            var l = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                if (Menu.Item("harassQ").GetValue<bool>())
                {
                    if (Player.Distance(l.Position) > 600 && (Q.IsReady()))
                    {
                        Q.CastOnBestTarget();
                    }

                }
        }

        private static void OnDoCast() => Obj_AI_Base.OnDoCast += (sender, args) =>
            {
                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                        || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (Menu.Item("useE").GetValue<bool>() && E.IsReady())
                            E.Cast();
                    }
                }
            };

        private static void Lane()
        {
            var laneE = Menu.Item("Laneclear Energy").GetValue<Slider>().Value;
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 290f);
            var minions = MinionManager.GetMinions(Player.ServerPosition, E.Range);
            if (minions.Count <= 2) return;
            {
                if (E.IsReady())
                {
                    if (
                        allMinions.Any(
                            minion =>
                            minion.IsValidTarget(E.Range)
                            && minion.Health < 1.10 * Player.GetSpellDamage(minion, SpellSlot.E)
                            && minion.IsValidTarget(E.Range)))
                    {
                        E.Cast();
                    }
                    if (Player.ManaPercent <= laneE) return;
                }
            }
        }

        private static void Jungle()
        {
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                E.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (Menu.Item("jungleclearE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.Cast();
                    }
                }
            }
        }
    }
}
