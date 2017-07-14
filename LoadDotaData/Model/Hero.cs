using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
        [JID("AttackCapabilities")]
        public AttackType AttackType { get; set; }

        [JID("AttackRange")]
        public decimal AttackRange { get; set; }

        [JID("ProjectileSpeed")]
        public decimal MissileSpeed { get; set; }

        [JID("MovementSpeed")]
        public int MoveSpeed { get; set; } = 300;

        [JID("MovementTurnRate")]
        public decimal TurnRate { get; set; }

        public decimal BaseHealth => 200;
        public decimal BaseMana => 75;

        [JID("StatusManaRegen")]
        public decimal BaseManaRegen { get; set; } = .01m;
        
        [JID("StatusHealthRegen")]
        public decimal BaseHealthRegen { get; set; }


        /// <summary>
        /// Attibutes
        /// </summary>
        [JID("AttributePrimary")]
        public StatType MainAttribute { get; set; }

        [JID("AttributeBaseStrength")]
        public decimal BaseStrength { get; set; }

        [JID("AttributeStrengthGain")]
        public decimal StrengthGain { get; set; }

        [JID("AttributeBaseAgility")]
        public decimal BaseAgi { get; set; }

        [JID("AttributeAgilityGain")]
        public decimal AgiGain { get; set; }

        [JID("AttributeBaseIntelligence")]
        public decimal BaseInt { get; set; }

        [JID("AttributeIntelligenceGain")]
        public decimal IntGain { get; set; }

        [JID("VisionNighttimeRange")]
        public decimal BaseNightVision { get; set; } = 800;

        [JID("VisionDaytimeRange")]
        public decimal BaseDayVision { get; set; } = 1800;

        [JID("RingRadius")]
        public decimal CollisionSize { get; set; }

        [JID("ArmorPhysical")]
        public decimal BaseArmor { get; set; } = -1;

        [Percentage]
        [JID("MagicalResistance")]
        public decimal BaseMagicResistance { get; set; }

        [JID("AttackDamageMin")]
        public decimal BaseDamageMin { get; set; }
        [JID("AttackDamageMax")]
        public decimal BaseDamageMax { get; set; }

        [JID("AttackRate")]
        public decimal BaseAttackTime { get; set; }

        [JID("AttackAnimationPoint")]
        public decimal AttackAnimation { get; set; }

        [JID("ProjectileSpeed")]
        public decimal ProjectileSpeed { get; set; } = 0;

        /// <summary>
        /// We just use this to collect a list of abilities and make parsing the ability file easier later
        /// </summary>
        [JID("Ability1", "Ability2", "Ability3", "Ability4", "Ability 5", "Ability6", "Ability7", "Ability8", "Ability9")]
        public string Ability { set => AbilityList.Add(value); } //accumulates all the different abilities
        public List<string> AbilityList { get; private set; } = new List<string>();
        public List<Ability> Abilities { get; set; } = new List<Ability>();

        /// <summary>
        /// This one is a little different because the order of the talents matters:
        /// 
        /// Ability 10: Lvl 10 Opt 1
        /// Ability 11: Lvl 10 Opt 2
        /// Ability 12: Lvl 15 Opt 1
        /// Ability 13: lvl 20 Opt 2
        /// 
        /// ....and so on
        /// </summary>
        [JID("Ability10", "Ability12", "Ability13", "Ability14", "Ability 15", "Ability16", "Ability17", "Ability11")]
        public string Talent { set => TalentList.Add(value); } //accumulates all the different abilities
        public List<string> TalentList { get; private set; } = new List<string>();
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
                case StatType.STRENGTH: return Strength(level);
                case StatType.AGILITY: return Agility(level);
                case StatType.INTELLIGENCE: return Intelligence(level);
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