using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    [ImageFolder("ability")]
    public class Ability : Parseable
    {
        [IgnoreDataMember]
        public override int MAX_ID => 9999;

        public string Description { get; set; }

        public int MaxLevels { get; set; } = 1; //how many ability points can be placed in this ability? //TODO: Should this just be read from attributes?

        [JID("AbilityType")]
        public AbilityType Ultimate { get; set; } = AbilityType.BASIC;

        [JID("AbilityCastRange")]
        public decimal[] CastRange { get; set; } = new decimal[] { 0 };

        [JID("AbilityCastPoint")]
        public decimal CastPoint { get; set; } = 0;

        [JID("AbilityCooldown")]
        public decimal[] Cooldown { get; set; } = new decimal[] { 0 };

        [JID("AbilityManaCost")]
        public decimal[] ManaCost { get; set; } = new decimal[] { 0 };

        [JID("AbilityChannelTime")]
        public decimal[] AbilityChannelTime { get; set; } = new decimal[] { 0 };

        public List<Effect> Effects { get; set; } = new List<Effect>();

        [JID("AbilityBehavior")]
        public EffectType Type { get; set; }

        public Effect ActiveEffect
        {
            get
            {
                var activeEffects = Effects.Where(e => e.IsActive);
                if (activeEffects.Count() == 1) return activeEffects.First();
                if (activeEffects.Count() == 0) return null;

                //TODO: stiffen up these parameters
                return activeEffects.FirstOrDefault();
            }
        }

        public Effect MainEffect => ActiveEffect ?? Effects.FirstOrDefault();
    }
}