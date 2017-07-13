using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    [DefaultEntryProperty(nameof(Amount))]
    public class Effect
    {
        /// <summary>
        /// Categorization of effect
        /// </summary>
        public EffectClass Class
        {
            get => effectClass;
            set
            {
                effectClass = value;
                IsActive = typeof(EffectClass).GetField(value.ToString()).GetCustomAttributes(typeof(ActiveEffect), false)?.FirstOrDefault() != null;
            }
        }
        private EffectClass effectClass = EffectClass.None;
        public bool IsActive { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Most classes have an amount; these are captured here with the exception of damage.
        /// 
        /// Percentages will be decimals (e.g. 50%=.50).
        /// </summary>
        public decimal[] Amount { get; set; } = new decimal[] { 0 };


        [JID("AbilityUnitTargetType")]
        public TargetType TargetType { get; set; } = TargetType.NONE;

        [JID("AbilityUnitTargetTeam")]
        public TargetTeam TargetTeam { get; set; } = TargetTeam.NONE;

        [JID("AbilityUnitDamageType")]
        public DamageType DamageType { get; set; } = DamageType.None;
        public DamageScalingType ScalingType { get; set; } = DamageScalingType.None;

        public DisableType DisableType { get; set; } = DisableType.None;        
        public bool DisjointsProjectiles { get; set; } = false;

        [JID("SpellImmunityType")]
        public SpellImmunityPiercingType PiercesSpellImmunity { get; set; } = SpellImmunityPiercingType.NO;

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
                DamageType = value?.Count() < 1 || value[0] == 0 ? DamageType.None
                                                                 : DamageType.PHYSICAL;            
            }
        }

        public bool HasAOE => AOE > 0;
        public decimal AOE { get; set; } = 0;

        public decimal CastAnimation { get; set; }
        public decimal CastAnimationFollowThrough { get; set; }
    }
}