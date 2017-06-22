using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;

namespace DotA.Model
{
    public class Hero
    {

        [InputHeader("HERO")] public string Name { get; set; }
        public byte[] Picture { get; set; }

        
        [InputHeader("RTCL")] public AttackType AttackType { get; set; }
        [InputHeader("RANG")] public AttackType AttackRange { get; set; }
        [InputHeader("MSPD")] public AttackType MissileSpeed { get; set; }


        [InputHeader("MOVE")] public int MoveSpeed { get; set; }
        [InputHeader("TURN")] public decimal TurnRate { get; set; }

        public decimal BaseHealth => 200;
        public decimal BaseMana => 75;
        public decimal BaseManaRegen => Name == "Techies" ? .02m : .01m;   //src: http://dota2.gamepedia.com/Mana         
        [InputHeader("HP/S")] public decimal BaseHealthRegen { get; set; }


        /// <summary>
        /// Attibutes
        /// </summary>
        [InputHeader("MAIN")] public MainAttribute MainAttribute { get; set; }
        [InputHeader("STRB")] public decimal BaseStrength { get; set; }
        [InputHeader("STR+")] public decimal StrengthGain { get; set; }
        [InputHeader("AGIB")] public decimal BaseAgi { get; set; }
        [InputHeader("AGI+")] public decimal AgiGain { get; set; }
        [InputHeader("INTB")] public decimal BaseInt { get; set; }
        [InputHeader("INT+")] public decimal IntGain { get; set; }

        [InputHeader("VS-N")] public int BaseNightVision { get; set; }
        [InputHeader("VS-D")] public int BaseDayVision { get; set; }

        [InputHeader("COLI")] public int CollisionSize { get; set; }

        [InputHeader("BSAR")] public decimal BaseArmor { get; set; }
        public decimal BaseMagicResistance //src: http://dota2.gamepedia.com/Magic_resistance
        {
            get
            {
                switch (Name)
                {
                    case "Meepo": return .35m;
                    case "Visage": return .1m;
                    default: return .25m;
                }
            }
        }

        [InputHeader("DMGM")] public int BaseDamageMin { get; set; }
        [InputHeader("DMGX")] public int BaseDamageMax { get; set; }

        [InputHeader("BAT_")] public decimal BaseAttackTime { get; set; }
        [InputHeader("BAT_")] public decimal AttackAnimation { get; set; }
        [InputHeader("BAT_")] public decimal AttackAnimationFollowThrough { get; set; }
        public decimal ProjectileSpeed { get; set; } = 0;

        public List<Ability> Abilities { get; set; } = new List<Ability>();
        public Talent[] Talents { get; set; } = new Talent[4];

        //Constants to calculate attributes
        const decimal HP_REGEN_PER_STR = .03m;
        const decimal HP_PER_STR = 20m;
        const decimal ARMOR_PER_AGI = .142857m;
        const decimal ATTACK_SPD_PER_AGI = 1;

        public decimal Strength(int level) => BaseStrength + (level * StrengthGain);
        public decimal Intelligence(int level) => BaseInt + (level * IntGain);
        public decimal Agility(int level) => BaseAgi + (level * AgiGain);

        public decimal MainAttributePoints(int level)
        {
            switch (MainAttribute)
            {
                case MainAttribute.Strength: return Strength(level);
                case MainAttribute.Agility: return Agility(level);
                case MainAttribute.Intelligence: return Intelligence(level);
                default: return 0m;
            }
        }

        public decimal HealthRegen(int level) => BaseHealthRegen + (HP_REGEN_PER_STR * Strength(level));
        public decimal Health(int level) => BaseHealth + (HP_PER_STR * Strength(level));

        public int AttackDamageMin(int level) => BaseDamageMin + (int)Math.Floor(MainAttributePoints(level));
        public int AttackDamageMax(int level) => BaseDamageMax + (int)Math.Ceiling(MainAttributePoints(level));
    }

    public class Talent
    {
        public int Level { get; set; }
        public Effect Option1 { get; set; }
        public Effect Option2 { get; set; }
    }
}