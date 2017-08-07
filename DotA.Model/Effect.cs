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
        public EffectClass Class { get; set; }

        [PrimaryKey] //this is cringey... this isn't really a primary key, I'm SO SORRY!
        public string ParentName { get; set; }

        [NoDisplay]
        public bool IsActive => typeof(EffectClass).GetField(Class.ToString())
                                                   .GetCustomAttributes(typeof(ActiveEffect), false)
                                                  ?.FirstOrDefault() != null;

        public string Description { get; set; }

        /// <summary>
        /// Most classes have an amount; these are captured here with the exception of damage.
        /// 
        /// Percentages will be decimals (e.g. 50%=.50).
        /// </summary>
        public List<decimal> Amount { get; set; } = new List<decimal>();

        //Potential level-scaling properties
        public List<decimal> Duration { get; set; } = new List<decimal>();

        /// <summary>
        /// This applies to things like Jinada and Bash where the effect is automatically triggered, so not a true cooldown
        /// </summary>
        public List<decimal> EffectResetTime = new List<decimal>();

        public List<decimal> Chance = new List<decimal>() { 1 };
        
        public List<decimal> BaseDamage { get; set; }

        [NoDisplay]
        public bool HasAOE => AOE > 0;
        public decimal AOE { get; set; } = 0;
    }
}