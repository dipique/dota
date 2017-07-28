using System;
using System.Collections.Generic;
using System.Linq;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    [DefaultEntryProperty(nameof(Amount))]
    [Serializable]
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
        public List<decimal> Amount { get; set; } = new List<decimal>();

        [JID("AbilityUnitDamageType")]
        public DamageType DamageType { get; set; } = DamageType.None;

        public DisableType DisableType { get; set; } = DisableType.None;        
        public bool DisjointsProjectiles { get; set; } = false;

        [JID("SpellImmunityType")]
        public SpellImmunityPiercingType PiercesSpellImmunity { get; set; } = SpellImmunityPiercingType.NO;

        //Potential level-scaling properties
        public List<decimal> Duration { get; set; } = new List<decimal>();

        /// <summary>
        /// This applies to things like Jinada and Bash where the effect is automatically triggered, so not a true cooldown
        /// </summary>
        public List<decimal> EffectResetTime = new List<decimal>();

        public List<decimal> Chance = new List<decimal>() { 1 };
        private List<decimal> baseDamage = new List<decimal>();
        public List<decimal> BaseDamage //defaults to physical
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
    }
}