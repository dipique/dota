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

        //Effect duration may actual be be stored at the ability level if not here.
        [JID("duration")]
        public List<decimal> Duration { get; set; } = new List<decimal>();

        /// <summary>
        /// This applies to things like Jinada and Bash where the effect is automatically triggered, so not a true cooldown
        /// </summary>
        public List<decimal> EffectResetTime = new List<decimal>();

        public List<decimal> Chance = new List<decimal>() { 1 };

        [JID("damage")]
        public List<decimal> Damage { get; set; } = new List<decimal>();

        public bool IsPassive { get; set; } = false;
    }
}

//locked off so it doesn't conflict while I'm writing it
namespace SomeOther
{
    public class EffectClass
    {
        public string Name { get; set; }
        public List<IndDestPair> ClassIndicators { get; set; } = new List<IndDestPair>();
        public List<IndDestPair> AssociatedEntries { get; set; } = new List<IndDestPair>();
        public bool FlipNegative { get; set; } = false;
        public bool ActiveEffect { get; set; } = false;
        public bool IsPercentage { get; set; } = false;

    }

    public struct IndDestPair //indicator-destination pairs (i.e. for this indicator, the field destination will be X)
    {
        public const string DEFAULT_DEST = nameof(DotA.Model.Effect.Amount);
        public string Indicator { get; set; }
        public string Destination { get; set; }

        public IndDestPair(string ind, string dest)
        {
            Indicator = ind;
            Destination = dest;
        }
    }

}