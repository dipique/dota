using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DotA_Flashcards.Droid.Model
{
    class Hero
    {
        public string Name { get; set; }

        public MainAttribute MainAttribute { get; set; }
        public AttackType RightClickAttack { get; set; }

        public int MoveSpeed { get; set; }
        public decimal TurnRate { get; set; }

        public decimal BaseHealth { get; set; }
        public decimal BaseHealthRegen { get; set; }

        public decimal BaseMana { get; set; }
        public decimal BaseManaRegen { get; set; }

        /// <summary>
        /// Attibutes
        /// </summary>
        public decimal BaseStrength { get; set; }
        public decimal StrengthGain { get; set; }
        public decimal BaseAgi { get; set; }
        public decimal AgiGain { get; set; }
        public decimal BaseInt { get; set; }
        public decimal IntGain { get; set; }

        public int BaseNightVision { get; set; }
        public int BaseDayVision { get; set; }

        public int CollisionSize { get; set; }

        public decimal BaseArmor { get; set; }

        public int BaseDamageMin { get; set; }
        public int BaseDamageMax { get; set; }

        public decimal BaseAttackTime { get; set; }
        
        public decimal ProjectileSpeed { get; set; }

        public decimal AttackAnimation { get; set; }

        public decimal AttackAnimationFollowThrough { get; set; }

        public decimal BaseMagicResistance { get; set; }


        //public List<Ability> Abilities = new List<Ability>();

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
}