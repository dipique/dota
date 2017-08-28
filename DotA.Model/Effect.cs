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
        [Options(SelectionOptions.EffectClass)]
        [DisplayOnly]
        public string Class { get; set; }

        public EffectClass GetEffectClass() => DotAData.ECs.FirstOrDefault(ec => ec.Name == Class);

        [DisplayOnly] //this is cringey... this isn't really a primary key, I'm SO SORRY!
        public string ParentName { get; set; }

        [PrimaryKey]
        [NoDisplay]
        public string ID => $"{ParentName}.{Class}";

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

public class EffectClass
{
    public const string DEF_NAME = "None";
    public string Name { get; set; } = DEF_NAME;
    public List<IndDestPair> ClassIndicators { get; set; } = new List<IndDestPair>();
    public List<IndDestPair> AssociatedEntries { get; set; } = new List<IndDestPair>();
    public bool FlipNegative { get; set; } = false;
    public bool ActiveEffect { get; set; } = false;
    public bool IsPercentage { get; set; } = false;

    public bool HasIndicator(string ind, out string dest)
    {
        var match = ClassIndicators.FirstOrDefault(i => i.Indicator == ind);
        if (match == null)
        {
            dest = null;
            return false;
        }

        dest = match.Destination;
        return true;
    }

}

//indicator-destination pairs (i.e. for this indicator, the field destination will be X)
public class IndDestPair 
{
    public const string DEFAULT_DEST = nameof(DotA.Model.Effect.Amount);
    public string Indicator { get; set; }
    public string Destination { get; set; }

    public IndDestPair(string ind, string dest)
    {
        Indicator = ind;
        Destination = dest;
    }

    public IndDestPair() { }
}