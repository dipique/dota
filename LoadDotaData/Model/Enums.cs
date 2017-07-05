using System;

namespace DotA.Model.Enums
{
    /// <summary>
    /// Indicates whether the ability damage is predicated on right click damage
    /// </summary>
    public enum DamageScalingType
    {
        None,
        HeroDamagePartial,  //base damage + normal attack
        HeroDamageFull      //only increases based on normal attack
    }

    public enum DisableType
    {
        None,
        Root,
        Stun
    }

    public enum DamageType
    {
        None,
        Physical,
        Magical,
        Pure
    }

    public enum LinkensInteraction
    {
        None,
        Pops,
        Pierces
    }

    public enum Preference
    {
        None,
        No,
        Partial,
        Yes
    }

    public enum Mobility
    {
        None,
        MovementSpeedIncrease,
        MaxMoveSpeed,
        Blink,
        Jump
    }

    public enum MainAttribute
    {
        Strength,
        Agility,
        Intelligence
    }

    public enum AttackType
    {
        Melee,
        Ranged
    }

    public enum EffectClass
    {
        Other = 0,

        [ValueDest("BaseDamage")]
        [JID("bonus_damage")]
        Damage,

        [ValueDest()]
        [JID("lifesteal_percent")]
        Lifesteal,

        [ValueDest()]
        [JID("movement_speed_percent_bonus")]
        Movement_Speed_Pct,

        [JID("slow")]
        [DurationTag("duration")]
        Slow,

        Disable,
        XP_Gain,

        [JID("bonus_cooldown")]
        Cooldown_Reduction,

        [JID("bonus_movement")]
        Movement_Speed,

        [JID("health_bonus")]
        Health,

        [JID("health_restore")]
        Health_Restore,

        [JID("mana_restore")]
        Mana_Restore,

        [JID("bonus_all_stats")]
        All_Stats,

        [JID("bonus_evasion")]
        [PercentEffect]
        Evasion,

        [JID("bonus_armor")]
        Armor,

        [PercentEffect]
        Spell_Amplification,

        [JID("bonus_attack_speed")]
        Attack_Speed,

        [JID("mana_restore")]
        Mana_Regen,

        [JID("bonus_mana")]
        Mana,

        [JID("bonus_intellect", "bonus_intelligence")]
        Intelligence,
        Cast_Range,

        [JID("bonus_magical_armor")]
        Magic_Resistance,

        GPM,
        Gold,

        [JID("bonus_strength")]
        Strength,
        Attack_Range,
        Agility,

        [ActiveEffect]
        [JID("health_restore")]
        Health_Regen,

        [PercentEffect]
        Spell_Lifesteal,

        [ActiveEffect]
        [JID("extra_spell_damage_precent")]
        [PercentEffect]
        Take_Extra_Magic_Damage,

        [ActiveEffect]
        [JID("stun_duration")]
        Stun,

        [JID("true_sight_range")]
        True_Sight,

        [ActiveEffect]
        [ExpectedValue("duration", "Duration")]
        [JID("fade_time", "fade_delay")]
        Invisibility,


        [JID("corruption_armor")]
        [DurationTag("curruption_duration")]
        Armor_Reduction
    }

    [Prefix("DOTA_ABILITY_BEHAVIOR_")]
    public enum EffectType //src: items.txt file
    {
        HIDDEN = 1,
        PASSIVE = 2,
        NO_TARGET = 4,
        UNIT_TARGET = 8,
        POINT = 16,
        AOE = 32,
        NOT_LEARNABLE = 64,
        CHANNELLED = 128,
        ITEM = 256,
        TOGGLE = 512,
        DIRECTIONAL = 1024,
        IMMEDIATE = 2048
    }

    [Prefix("DOTA_UNIT_TARGET_")]
    public enum TargetType //src: npc_abilities.txt
    {
        NONE = 0,
        FRIENDLY_HERO = 5,
        FRIENDLY_BASIC = 9,
        FRIENDLY = 13,
        ENEMY_HERO = 6,
        ENEMY_BASIC = 10,
        ENEMY = 14,
        ALL = 15,
    }

    /// <summary>
    /// By default, the option text will just be the enum text. Hierarchies are
    /// separated by underscore. Optionally, other text can be added for display. These
    /// are also separated by underscore if hierarchical.
    /// </summary>
    public class OptionText : Attribute
    {
        public string Description { get; set; }
        public OptionText(string desc)
        {
            Description = desc;
        }
    }

    /// <summary>
    /// JSON identifiers for certain elements
    /// </summary>
    public class JID : Attribute
    {
        public string[] IDs { get; set; }
        public JID(params string[] ids)
        {
            IDs = ids;
        }
    }

    /// <summary>
    /// For enums defined in Dota already, the prefix is removed from each value for easier coding. It is kept to make
    /// it easier to parse files.
    /// </summary>
    public class Prefix : Attribute
    {
        public string Value { get; set; }
        public Prefix(string val)
        {
            Value = val;
        }
    }

    /// <summary>
    /// Determines which field should store the value provided. If blank it will be
    /// stored in the "Amount" field.
    /// </summary>
    public class ValueDest : Attribute
    {
        public string FieldName { get; set; }
        public ValueDest(string field = "Amount")
        {
            FieldName = field;
        }
    }

    /// <summary>
    /// For this effect class, we expect to see certain other attributes
    /// </summary>
    public class ExpectedValue : Attribute
    {
        public string Indicator { get; set; }
        public string DestField { get; set; }
        public ExpectedValue(string ind, string dest)
        {
            Indicator = ind;
            DestField = dest;
        }
    }

    public class InputHeader: Attribute
    {
        public string Header { get; set; }
        public InputHeader(string header)
        {
            Header = header;
        }
    }

    public class ImageFolder : Attribute
    {
        public string FolderName { get; set; }
        public ImageFolder(string folder)
        {
            FolderName = folder;
        }
    }

    /// <summary>
    /// Indicates that a given effect should be assumed to be an active (passive is the default)
    /// </summary>
    public class ActiveEffect : Attribute
    {
        public ActiveEffect() { }
    }

    /// <summary>
    /// Indicates that a given effect value is a percentage
    /// </summary>
    public class PercentEffect : Attribute
    {
        public PercentEffect() { }
    }

    /// <summary>
    /// Some effects store their duration in a separate entry; it can be specified using this attribute
    /// </summary>
    public class DurationTag : Attribute
    {
        public string Tag { get; set; }
        public DurationTag(string tag) { Tag = tag; }
    }
}