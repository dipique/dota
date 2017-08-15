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

        public bool IsPassive => EffectType.HasFlag(EffectType.PASSIVE);

        /// <summary>
        /// This may be projectile speed OR expansion speed (like ravage)
        /// </summary>
        [JID("bolt_speed", "blast_speed", "speed", "wave_speed")]
        public List<decimal> ProjectileSpeed { get; set; } = new List<decimal>();

        [JID("radius", "bolt_aoe")]
        public List<decimal> Radius { get; set; } = new List<decimal>();

        [JID("AbilityDuration")]
        public List<decimal> Duration { get; set; } = new List<decimal>();

        [SpecialHandlerSectionMethod("AbilitySpecial")]
        public void ParseAbilitySpecial(Section s)
        {
            ParseAbilitySpecial(s, Effects, this);

            //Set the effects to passive or active. The logic is:
            //  1. If the ability is passive, so are the effects
            //  2. If not, the effect is active if that ability type is active (decorated in the enum).
            //  3. Otherwise the effect is passive.
            //The enum decorations have already been applied, so here we just make sure there aren't active
            //effects inside passive abilities.
            if (IsPassive) Effects.ForEach(e => e.IsPassive = true);
        }
    }
}