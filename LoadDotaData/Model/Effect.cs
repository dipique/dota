using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;

namespace DotA.Model
{
    public class Effect
    {
        /// <summary>
        /// Categorization of effect
        /// </summary>
        public EffectClass Class { get; set; }

        /// <summary>
        /// Passive or active
        /// </summary>
        public EffectType Type { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Most classes have an amount; these are captured here with the exception of damage.
        /// 
        /// Percentages will be decimals (e.g. 50%=.50).
        /// </summary>
        public decimal Amount { get; set; }

        public TargetType TargetType { get; set; } = TargetType.NONE;
        public DamageType DamageType { get; set; } = DamageType.None;
        public DamageScalingType ScalingType { get; set; } = DamageScalingType.None;

        public DisableType DisableType { get; set; } = DisableType.None;        
        public bool DisjointsProjectiles { get; set; } = false;

        public bool PiercesSpellImmunity { get; set; } = false;
        public LinkensInteraction LinkensInteraction { get; set; } = LinkensInteraction.None;

        //Potential level-scaling properties
        public decimal[] Duration { get; set; } = new decimal[] { 0 };
        public decimal[] CastRange { get; set; } = new decimal[] { 0 };
        public int[] Cooldown { get; set; } = new int[] { 0 };
        public decimal[] ManaCost { get; set; } = new decimal[] { 0 };
        private decimal[] baseDamage = new decimal[] { 0 };
        public decimal[] BaseDamage //defaults to physical
        {
            get => baseDamage;
            set
            {
                baseDamage = value;
                if (value == null || value.Count() == 0 || value[0] == 0)
                    DamageType = DamageType.None;
                else
                    DamageType = DamageType.Physical;                
            }
        }

        /// <summary>
        /// Effects can be "leveled up" for abilities, but not for items. These methods
        /// offer an easy way to specify a level or omit it; the default gets the first value
        /// in the array, which will be the only value if this is an item effect.
        /// </summary>
        public decimal GetCastRange(int points = 1)
        {
            if (CastRange.Count() < points) return 0;
            return CastRange[points - 1];
        }
        public decimal GetBaseDamage(int points = 1)
        {
            if (BaseDamage.Count() < points) return 0;
            return BaseDamage[points - 1];
        }
        public decimal GetCooldown(int points = 1)
        {
            if (Cooldown.Count() < points) return 0;
            return Cooldown[points - 1];
        }
        public decimal GetDuration(int points = 1)
        {
            if (Duration.Count() < points) return 0;
            return Duration[points - 1];
        }

        public bool HasAOE => AOE > 0;
        public decimal AOE { get; set; } = 0;

        public decimal CastAnimation { get; set; }
        public decimal CastAnimationFollowThrough { get; set; }
    }
}