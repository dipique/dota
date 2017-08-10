using System.Collections.Generic;
using System.Linq;
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
        public override bool RequiresID => false;

        public string Description { get; set; }

        [NoDisplay]
        public int MaxLevels
        {
            get
            {
                //Check the properties here for the highest upper bound
                int maxCount = 1;
                foreach (var p in typeof(Ability).GetProperties().Where(p => p.PropertyType.IsArray))
                {
                    var countMethod = ((decimal[])p.GetValue(this))?.Count() ?? 0;
                    if (countMethod > maxCount) maxCount = countMethod;
                }

                //Now check the effects
                foreach (var p in typeof(Effect).GetProperties().Where(p => p.PropertyType.IsArray))
                {                    
                    Effects.ForEach(e => {
                        var countMethod = ((decimal[])p.GetValue(e))?.Count() ?? 0;
                        if (countMethod > maxCount) maxCount = countMethod;
                    });
                }

                return maxCount;
            }
            set { return; }
        }

        [JID("AbilityType")]
        public AbilityType AbilityType { get; set; } = AbilityType.BASIC;

        [JID("AbilityUnitDamageType")]
        public DamageType DamageType { get; set; } = DamageType.NONE;

        [JID("AbilityDamage")]
        public List<decimal> Damage { get; set; } = new List<decimal>();

        [JID("SpellImmunityType")]
        public SpellImmunityPiercingType PiercesSpellImmunity { get; set; } = SpellImmunityPiercingType.ENEMIES_NO;

        [JID("AbilityCastRange")]
        public List<decimal> CastRange { get; set; } = new List<decimal>();

        [JID("AbilityCastPoint")]
        public decimal CastPoint { get; set; } = 0;

        [JID("AbilityCooldown")]
        public List<decimal> Cooldown { get; set; } = new List<decimal>();

        [JID("AbilityManaCost")]
        public List<decimal> ManaCost { get; set; } = new List<decimal>();

        [JID("AbilityChannelTime")]
        public List<decimal> AbilityChannelTime { get; set; } = new List<decimal>();

        [JID("AbilityUnitTargetType")]
        public TargetType TargetType { get; set; } = TargetType.NONE;

        [JID("AbilityUnitTargetTeam")]
        public TargetTeam TargetTeam { get; set; } = TargetTeam.NONE;

        public List<Effect> Effects { get; set; } = new List<Effect>();

        [JID("AbilityBehavior")]
        public EffectType EffectType { get; set; }

        /// <summary>
        /// This may be projectile speed OR expansion speed (like ravage)
        /// </summary>
        [JID("bolt_speed", "blast_speed", "speed", "wave_speed")]
        public List<decimal> ProjectileSpeed { get; set; } = new List<decimal>();

        [JID("radius", "bolt_aoe")]
        public List<decimal> Radius { get; set; } = new List<decimal>();

        [JID("AbilityDuration")]
        public List<decimal> Duration { get; set; } = new List<decimal>();

        /// <summary>
        /// Here's how this assigns effects:
        /// 
        /// 1. Loop through all entries that have a uniquely assigned effectclass via JID (must be at least one or
        ///    it wouldn't know what effect there was)
        /// 2. Takes entries that are listed as expected entries and assign them
        /// 3. Ignores unassigned entries
        /// </summary>
        /// <param name="s"></param>
        [SpecialHandlerSectionMethod("AbilitySpecial")]
        public void ParseAbilitySpecial(Section s)
        {
            var entries = s.GetAllEntries();

            //Every entry with a JID will have it own effect.
            foreach (var entry in entries.Where(e => e.AssociatedEffectClass != EffectClass.None))
            {
                var effect = new Effect() {
                    Class = entry.AssociatedEffectClass,
                    ParentName = Name
                };

                //set property to the entry value--effect first, then the base item
                entry.Apply(effect, this);

                //Now, get any entries associated with it
                foreach (var associatedEntry in entries.Where(e => e.AssociatedEffectClass == EffectClass.None)
                                                       .Where(e => entry.ExpectedEntries.Select(ee => ee.name).Contains(e.Title)))
                {
                    associatedEntry.ValueDest = entry.ExpectedEntries.First(ee => ee.name == associatedEntry.Title).dest;
                    entry.Apply(effect, this);
                }

                //Add the entry
                Effects.Add(effect);
            }
        }
    }
}