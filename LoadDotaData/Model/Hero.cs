using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    /// <summary>
    ///  Located in the game\dota\scripts\npc folder inside your Dota 2 install directory
    /// </summary>
    [ImageFolder("hero")]
    public class Hero : Parseable
    {

        private const string HERO_NAME_TECHIES = "Techies";

        public AttackType AttackType { get; set; }

        [JID("AttackRange")]
        public AttackType AttackRange { get; set; }

        [JID("ProjectileSpeed")]
        public AttackType MissileSpeed { get; set; }

        public int MoveSpeed { get; set; } = 300;
        public decimal TurnRate { get; set; }

        public decimal BaseHealth => 200;
        public decimal BaseMana => 75;
        public decimal BaseManaRegen => Name == HERO_NAME_TECHIES ? .02m : .01m;   //src: http://dota2.gamepedia.com/Mana         
        public decimal BaseHealthRegen { get; set; }


        /// <summary>
        /// Attibutes
        /// </summary>
        public MainAttribute MainAttribute { get; set; }

        [JID("AttributePrimary")]
        public string SetMainAttribute
        {
            set
            {

            }
        }

        public decimal BaseStrength { get; set; }
        public decimal StrengthGain { get; set; }
        public decimal BaseAgi { get; set; }
        public decimal AgiGain { get; set; }
        public decimal BaseInt { get; set; }
        public decimal IntGain { get; set; }

        public decimal BaseNightVision { get; set; } = 800;
        public decimal BaseDayVision { get; set; } = 1800;

        public decimal CollisionSize { get; set; }

        [JID("ArmorPhysical")]
        public decimal BaseArmor { get; set; } = -1;
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

        [JID("AttackDamageMin")]
        public decimal BaseDamageMin { get; set; }
        [JID("AttackDamageMax")]
        public decimal BaseDamageMax { get; set; }

        [JID("AttackRate")]
        public decimal BaseAttackTime { get; set; }

        [JID("AttackAnimationPoint")]
        public decimal AttackAnimation { get; set; }
        public decimal AttackAnimationFollowThrough { get; set; }
        public decimal ProjectileSpeed { get; set; } = 0;

        public List<Ability> Abilities { get; set; } = new List<Ability>();
        public Talent[] Talents { get; set; } = new Talent[4];
        public List<string> AbilityList { get; private set; } = new List<string>();

        [JID("Ability1", "Ability2", "Ability3", "Ability4", "Ability 5", "Ability6", "Ability7", "Ability8", "Ability9")]
        public string Ability { set => AbilityList.Add(value); } //accumulates all the different abilities

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

        public decimal AttackDamageMin(int level) => BaseDamageMin + Math.Floor(MainAttributePoints(level));
        public decimal AttackDamageMax(int level) => BaseDamageMax + Math.Ceiling(MainAttributePoints(level));
    }

    public class Talent
    {
        public int Level { get; set; }
        public Effect Option1 { get; set; }
        public Effect Option2 { get; set; }
    }
}