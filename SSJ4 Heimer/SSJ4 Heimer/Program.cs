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

namespace SSJ4_Heimer
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

        //private static Obj_AI_Hero Player;
        
        
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            
        }
        
        
        static void Game_OnGameLoad(EventArgs args)
        {
            //Player = ObjectManager.Player;
            
            
            
        	if (ObjectManager.Player.BaseSkinName != Champion) return;

            
            
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 525);

            W.SetSkillshot(250f, 200, 1400, true, SkillshotType.SkillshotLine);
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
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("AutoR", "Auto R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("CountR", "Num of Enemy in Range to Ult").SetValue(new Slider(1, 5, 0)));
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            
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
           

        }
        
        private static void Combo()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;
			
            //Combo
            if (target.IsValidTarget(Q.Range) && Q.IsReady() && (Config.Item("UseQCombo").GetValue<bool>()))
            {
                
                    Q.Cast(target);

            }
            if (W.IsReady() && (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1) && (Config.Item("UseWCombo").GetValue<bool>()))
                if (target.IsValidTarget(W.Range) && W.IsReady() && (Config.Item("UseWCombo").GetValue<bool>()))
            {
                
                    W.Cast(target);

            }
            
            if (target.IsValidTarget(E.Range) && E.IsReady() && (Config.Item("UseECombo").GetValue<bool>()))
            {
               
                    E.Cast(target);
                
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
            if (Config.Item("AutoR").GetValue<bool>())
            {

                
            }


        }
        
        
        
        private static void OnDraw(EventArgs args) 
        {
            if (Config.Item("CircleLag").GetValue<bool>()) // Credits to SKOBOL
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
                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White,
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
                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White);
                }

            }
        }
		
	}
}