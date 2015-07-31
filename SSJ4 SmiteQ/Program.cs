

namespace SSJ4_SmiteQ
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class Program
    {


        private static List<Spell> SpellList = new List<Spell>();

        private static Spell Q;

        private static Spell smite;

        private static Menu Config;

        public static double damage;

        public static string Champ;

        private static int Plevel;

        public static SpellSlot Smite = ObjectManager.Player.GetSpellSlot("SummonerSmite");

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            if (Player.BaseSkinName == "LeeSin")
            {
                Q = new Spell(SpellSlot.Q, 1100);
                Q.SetSkillshot(0.5f, 60f, 1500f, true, SkillshotType.SkillshotLine);
                Champ = "LeeSin";
            }
            else if (Player.BaseSkinName == "Blitzcrank")
            {
                Q = new Spell(SpellSlot.Q, 1000);
                Q.SetSkillshot(0.5f, 70f, 1800f, true, SkillshotType.SkillshotLine);
                Champ = "Blitzcrank";
            }
            else
            {
                return;
            }

            smite = new Spell(Smite, 500);

            Config = new Menu("SSJ4 SmiteQ", "SSJ4 SmiteQ", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddItem(new MenuItem("qSmite", "Q->Smite")).SetValue(new KeyBind(32, KeyBindType.Press));

            Config.AddToMainMenu();
            int level = ObjectManager.Player.Level;
            Plevel = level;
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.PrintChat("SSJ4 SmiteQ loaded. Champion selected: " + Champ);
        }

        private static void OnGameUpdate(EventArgs args)
        {
            smiteDmg();
            if (ObjectManager.Player.Level > Plevel)
            {
                Plevel = ObjectManager.Player.Level;
                smiteDmg();
            }

            if (Config.Item("qSmite").GetValue<KeyBind>().Active)
            {
                smiteQ();
            }

        }

        public static void smiteQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var prediction = Q.GetPrediction(target);

            if (Player.IsDead) return;

            var pos1 = Drawing.WorldToScreen(Player.Position);
            var pos2 = Drawing.WorldToScreen(target.Position);

            if (Config.Item("qSmite").GetValue<KeyBind>().Active)
            {

                smiteDmg();

                if (target == null) return;

                var state = Q.Cast(target);
                if (state.IsCasted())
                {
                    return;
                }

                if (state == Spell.CastStates.Collision)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CollisionObjects.Count(i => i.IsValid<Obj_AI_Minion>() && i.IsEnemy) == 1)
                    {
                        if (Player.Distance(pred.CollisionObjects.First()) > 520)
                        {
                            return;
                        }
                        Player.Spellbook.CastSpell(smite.Slot, pred.CollisionObjects.First());
                        Q.Cast(pred.CastPosition);
                        return;
                    }
                }
            }
        }

        public static void smiteDmg()
        {
            int level = ObjectManager.Player.Level;
            int[] smitedamage = { 20 * level + 370, 30 * level + 330, 40 * level + 240, 50 * level + 100 };
            damage = smitedamage.Max();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            
        }

    }
}