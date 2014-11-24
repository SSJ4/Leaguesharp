﻿#region
using System;
using System.Collections;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace SSJ4_Heimerdinger
{


    internal class program
    {

        private const string Champion = "Heimerdinger";

        private static Orbwalking.Orbwalker Orbwalker;

        private static List<Spell> SpellList = new List<Spell>();

        private static Spell Q;

        private static Spell W;

        private static Spell E;

        private static Spell R;

        private static Menu Config;

        private static Items.Item RDO;

        private static Items.Item DFG;

        private static Items.Item YOY;

        private static Items.Item BOTK;

        private static Items.Item HYD;

        private static Items.Item CUT;

        private static Items.Item TYM;

        private static Items.Item ZHO;

        private static List<Vector3> WardSpots;


        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;

        }


        static void Game_OnGameLoad(EventArgs args)
        {

            if (ObjectManager.Player.BaseSkinName != Champion) return;



            Q = new Spell(SpellSlot.Q, 525);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);


            W.SetSkillshot(250f, 200, 1400, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.51f, 120, 1200, false, SkillshotType.SkillshotCircle);


            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);


            RDO = new Items.Item(3143, 490f);
            HYD = new Items.Item(3074, 175f);
            DFG = new Items.Item(3128, 750f);
            YOY = new Items.Item(3142, 185f);
            BOTK = new Items.Item(3153, 450f);
            CUT = new Items.Item(3144, 450f);
            TYM = new Items.Item(3077, 175f);
            ZHO = new Items.Item(3157, 1f);

            //Menu
            Config = new Menu(Champion, "SSJ4 Heimerdinger", true);

            //Ts
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalk
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo Menu
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            //Config.SubMenu("Combo").AddItem(new MenuItem("posPrint", "Print position!").SetValue(new KeyBind(32, KeyBindType.Press)));


            //KS Menu
            Config.AddSubMenu(new Menu("KS Menu", "KSMenu"));
            Config.SubMenu("KSMenu").AddItem(new MenuItem("rwKS", "Use R->W for KS")).SetValue(true);
            Config.SubMenu("KSMenu").AddItem(new MenuItem("KSW", "Use W")).SetValue(true);
            Config.SubMenu("KSMenu").AddItem(new MenuItem("KSE", "Use E")).SetValue(true);

            //Safe Menu
            Config.AddSubMenu(new Menu("Safe me!", "SafeMenu"));
            Config.SubMenu("SafeMenu").AddItem(new MenuItem("ZhoUlt", "Zhonyas Turret Ult")).SetValue(true);

            //Turret spot drawings
            Config.AddSubMenu(new Menu("Turret Management", "drawTur"));
            Config.SubMenu("drawTur").AddItem(new MenuItem("DrawSpots", "Draw turret spots")).SetValue(true);
            Config.SubMenu("drawTur").AddItem(new MenuItem("TurOnSpot", "Place Turret on spot")).SetValue(new KeyBind(32, KeyBindType.Press));

            //Range Drawings
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawEnable", "Enable Drawing"));

            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;

        }

        private static void OnGameUpdate(EventArgs args)
        {




            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (Config.Item("rwKS").GetValue<bool>())
            {
                rwKSCombo();
            }

            //if (Config.Item("rE").GetValue<bool>())
            //{
            //	rECombo();
            // }

            if (Config.Item("KSW").GetValue<bool>())
            {
                KSW();
            }

            if (Config.Item("ZhoUlt").GetValue<bool>())
            {
                ZhoUlt();
            }

            /*if (Config.Item("posPrint").GetValue<KeyBind>().Active)
            {
             var curPos = ObjectManager.Player.Position;
           	
             Game.PrintChat(curPos.ToString());
            }*/

            TurretSpots();



        }

        public static void TurretSpots()
        {
            WardSpots = new List<Vector3>();

            WardSpots.Add(new Vector3(7456f, 7330f, 53.83824f));
            WardSpots.Add(new Vector3(7252f, 7560f, 54.31723f));
            WardSpots.Add(new Vector3(7694f, 7196f, 53.62105f));
        }


        private static void ZhoUlt()
        {
            var CurrHP = ObjectManager.Player.Health;
            var FullHP = ObjectManager.Player.MaxHealth;
            var CritHP = FullHP / 100 * 20;

            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;



            if (CurrHP <= CritHP)
            {

                if (Q.IsReady() && R.IsReady())
                {
                    R.CastOnUnit(ObjectManager.Player);
                    Utility.DelayAction.Add(100, () => Q.Cast(Player.Position));
                    Utility.DelayAction.Add(500, () => ZHO.Cast(ObjectManager.Player));
                }

            }

        }


        private static void Combo()
        {


            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            //var collisionObjects = LeagueSharp.Common.Collision.GetCollision(new List<Vector3> { predictedCastPosition }, new PredictionInput { Delay = 250f, Radius = 200, Speed = 1400 });



            //Combo
            if (W.IsReady() && (Config.Item("UseWCombo").GetValue<bool>()))
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    W.Cast(prediction.CastPosition);

                }
            }
            if (target.IsValidTarget(E.Range) && E.IsReady() && (Config.Item("UseECombo").GetValue<bool>()))
            {
                E.Cast(target, true, true);
            }






            if (Config.Item("UseItems").GetValue<bool>())
            {
                if (Player.Distance(target) <= RDO.Range)
                {
                    RDO.Cast(target);
                }
                if (Player.Distance(target) <= HYD.Range)
                {
                    HYD.Cast(target);
                }
                if (Player.Distance(target) <= DFG.Range)
                {
                    DFG.Cast(target);
                }
                if (Player.Distance(target) <= BOTK.Range)
                {
                    BOTK.Cast(target);
                }
                if (Player.Distance(target) <= CUT.Range)
                {
                    CUT.Cast(target);
                }
                if (Player.Distance(target) <= 125f)
                {
                    YOY.Cast();
                }
                if (Player.Distance(target) <= TYM.Range)
                {
                    TYM.Cast(target);
                }
            }



        }


        private static void KSW()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            var prediction = W.GetPrediction(target);

            if (W.IsReady())
            {

                if (target.Health < GetWDamage(target))
                {
                    if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 3)
                    {
                        Utility.DelayAction.Add(100, () => W.Cast(prediction.CastPosition));
                    }


                }
            }
        }

        private static void KSE()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            var prediction = E.GetPrediction(target);

            if (W.IsReady())
            {

                if (target.Health < GetEDamage(target))
                {
                    if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 3)
                    {
                        Utility.DelayAction.Add(100, () => E.Cast(prediction.CastPosition));
                    }


                }
            }
        }

        private static void rwKSCombo()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            var prediction = W.GetPrediction(target);

            if (W.IsReady() && R.IsReady())
            {

                if (target.Health < GetRwDamage(target))
                {
                    if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 3)
                    {
                        R.CastOnUnit(ObjectManager.Player);

                        Utility.DelayAction.Add(100, () => W.Cast(prediction.CastPosition));
                    }


                }
            }
        }

        private static void rECombo()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            var prediction = E.GetPrediction(target);

            if (R.IsReady() && E.IsReady() && (Config.Item("UseRCombo").GetValue<bool>()) && (Config.Item("UseECombo").GetValue<bool>()))
            {

                if (prediction.Hitchance >= HitChance.High && prediction.AoeTargetsHit.Count(h => h.IsEnemy && !h.IsDead) >= 2)
                {
                    R.CastOnUnit(ObjectManager.Player);
                    Utility.DelayAction.Add(100, () => E.Cast(prediction.CastPosition));
                }
            }

        }


        private static float GetWDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (DFG.IsReady())
                damage = damage * 1.2;


            return (float)damage;
        }


        private static float GetEDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (DFG.IsReady())
                damage = damage * 1.2;


            return (float)damage;
        }



        private static float GetRwDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W, 1);

            if (DFG.IsReady())
                damage = damage * 1.2;


            return (float)damage;
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) + Player.GetSpellDamage(enemy, SpellSlot.Q, 1);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 8;

            if (DFG.IsReady())
                damage = damage * 1.2;



            return (float)damage;
        }



        private static void OnDraw(EventArgs args)
        {


            if (Config.Item("DrawSpots").GetValue<bool>())
            {


                foreach (Vector3 wardPos in WardSpots)
                {
                    var MousePos = Game.CursorPos.Distance(wardPos);

                    if (ObjectManager.Player.Distance(wardPos) < 2000)
                    {
                        if (MousePos < 100)
                        {
                            Utility.DrawCircle(wardPos, 100, Color.Red, 5, 5, false);

                            if (Config.Item("TurOnSpot").GetValue<KeyBind>().Active)
                            {
                                if (ObjectManager.Player.Position.Distance(wardPos) <= 525 && Q.IsReady())
                                {
                                    Q.Cast(wardPos);
                                }
                                else
                                {
                                    Player.IssueOrder(GameObjectOrder.MoveTo, wardPos);
                                }
                            }


                        }
                        else
                        {
                            Utility.DrawCircle(wardPos, 100, Color.Aqua, 5, 5, false);
                        }
                    }
                }



            }


            #region Turretdraw

            #endregion
            #region RangeDraw
            if (Config.Item("DrawEnable").GetValue<bool>())
            {
                if (Config.Item("CircleLag").GetValue<bool>())
                {
                    if (Config.Item("DrawQ").GetValue<bool>())
                    {
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White,
                            Config.Item("CircleThickness").GetValue<Slider>().Value,
                            Config.Item("CircleQuality").GetValue<Slider>().Value);
                    }
                    if (Config.Item("DrawW").GetValue<bool>())
                    {
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White,
                            Config.Item("CircleThickness").GetValue<Slider>().Value,
                            Config.Item("CircleQuality").GetValue<Slider>().Value);
                    }
                    if (Config.Item("DrawE").GetValue<bool>())
                    {
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White,
                            Config.Item("CircleThickness").GetValue<Slider>().Value,
                            Config.Item("CircleQuality").GetValue<Slider>().Value);
                    }
                }
                else
                {
                    if (Config.Item("DrawQ").GetValue<bool>())
                    {
                        Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White);
                    }
                    if (Config.Item("DrawW").GetValue<bool>())
                    {
                        Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White);
                    }
                    if (Config.Item("DrawE").GetValue<bool>())
                    {
                        Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White);
                    }
                }





            }
            #endregion
        }


    }
}