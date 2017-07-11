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

        public string Description { get; set; }

        /// <summary>
        /// Most classes have an amount; these are captured here with the exception of damage.
        /// 
        /// Percentages will be decimals (e.g. 50%=.50).
        /// </summary>
        public decimal[] Amount { get; set; } = new decimal[] { 0 };

        public TargetType TargetType { get; set; } = TargetType.NONE;
        public DamageType DamageType { get; set; } = DamageType.None;
        public DamageScalingType ScalingType { get; set; } = DamageScalingType.None;

        public DisableType DisableType { get; set; } = DisableType.None;        
        public bool DisjointsProjectiles { get; set; } = false;

        public bool PiercesSpellImmunity { get; set; } = false;
        public LinkensInteraction LinkensInteraction { get; set; } = LinkensInteraction.None;

        //Potential level-scaling properties
        public decimal[] Duration { get; set; } = new decimal[] { 0 };

        /// <summary>
        /// This applies to things like Jinada and Bash where the effect is automatically triggered, so not a true cooldown
        /// </summary>
        public decimal[] EffectResetTime { get; set; } = new decimal[] { 0 };

        public decimal[] Chance { get; set; } = new decimal[] { 1 };
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

        public bool HasAOE => AOE > 0;
        public decimal AOE { get; set; } = 0;

        public decimal CastAnimation { get; set; }
        public decimal CastAnimationFollowThrough { get; set; }
    }
}