using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Threading;

namespace SSJ4_Heimerdinger
{
    internal class Program
    {
        private const string Champion = "Heimerdinger";
        private static Orbwalking.Orbwalker Orbwalker;
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell Q1;
        private static Spell W1;
        private static Spell E1;
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
        private static List<Vector3> TurretSpots;

        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != Champion) return;

            Q = new Spell(SpellSlot.Q, 525);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925 - 100);
            R = new Spell(SpellSlot.R, 100);

            W1 = new Spell(SpellSlot.W, 1100);
            E1 = new Spell(SpellSlot.E, 925 - 100);

            Q.SetSkillshot(0.5f, 40f, 1100f, true, SkillshotType.SkillshotLine);

            W.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            W1.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E1.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);

            RDO = new Items.Item(3143, 490f);
            HYD = new Items.Item(3074, 175f);
            DFG = new Items.Item(3128, 750f);
            YOY = new Items.Item(3142, 185f);
            BOTK = new Items.Item(3153, 450f);
            CUT = new Items.Item(3144, 450f);
            TYM = new Items.Item(3077, 175f);
            ZHO = new Items.Item(3157, 1f);

            //Menu
            Config = new Menu(Champion, "SSJ4 Heimerdinger 2.0", true);

            //Targetselector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalk
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("KS", "Killsteal")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ZhoUlt", "Ult + Q > Zhonyas")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            //Drawing.OnDraw += OnDraw;

            Game.PrintChat("Welcome to SSJ4 Heimerdinger 2.0");
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (Config.Item("KS").GetValue<bool>())
            {
                KS();
            }
            if (Config.Item("ZhoUlt").GetValue<bool>())
            {
                ZhoUlt();
            }


        }



        private static void ZhoUlt()
        {
            var fullHP = Player.MaxHealth;
            var HP = Player.Health;
            var critHP = fullHP / 4;
            if (HP <= critHP)
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
                if (target == null) return;
                R.Cast();
                Utility.DelayAction.Add(1010, () => Q.Cast(Player.Position));
                Utility.DelayAction.Add(500, () => Q.Cast(Player.Position));
                Utility.DelayAction.Add(100, () => ZHO.Cast());
            }

        }


        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;

             if (Config.Item("UseItems").GetValue<bool>())
           
                if (Player.Distance3D(target) <= RDO.Range)
                {
                    RDO.Cast(target);
                }
                if (Player.Distance3D(target) <= HYD.Range)
                {
                    HYD.Cast(target);
                }
                if (Player.Distance3D(target) <= DFG.Range)
                {
                    DFG.Cast(target);
                }
                if (Player.Distance3D(target) <= BOTK.Range)
                {
                    BOTK.Cast(target);
                }
                if (Player.Distance3D(target) <= CUT.Range)
                {
                    CUT.Cast(target);
                }
                if (Player.Distance3D(target) <= 125f)
                {
                    YOY.Cast();
                }
                if (Player.Distance3D(target) <= TYM.Range)
                {
                    TYM.Cast(target);
                }
            
            
            if (E.IsReady() && Config.Item("UseECombo").GetValue<bool>())
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
                E.CastIfHitchanceEquals(target, HitChance.High, true);
            }

            target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (W.IsReady() && Config.Item("UseWCombo").GetValue<bool>())
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    if (target.Health < GetW1Damage(target))
                    {
                        R.Cast();
                        W.Cast(prediction.CastPosition);
                        W.Cast(prediction.CastPosition);
                    }
                    else
                    {
                    W.Cast(prediction.CastPosition);
                    }
                }
                
            }
        }

        private static void KS()
        {
            var target = TargetSelector.GetTarget(E.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetEDamage(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
                E.CastIfHitchanceEquals(target, HitChance.High, true);
                return;
            }


            target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetWDamage(target))
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {

                    W.Cast(prediction.CastPosition);
                    return;
                }
            }

            target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetW1Damage(target) && R.IsReady())
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    R.Cast();
                    W.Cast(prediction.CastPosition);
                    W.Cast(prediction.CastPosition);
                    return;
                }
            }

        }

        private static float GetWDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.W);

            return (float)damage * 2;
        }

        private static float GetW1Damage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W1.IsReady() && R.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.W, 1);

            return (float)damage * 2;
        }

        private static float GetEDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (E.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.E);

            return (float)damage * 2;
        }


    }
}