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
        [ExpectedValue("duration", "Duration")]
        Slow,

        Disable,
        XP_Gain,
        Cooldown_Reduction,
        Movement_Speed,

        [JID("health_bonus")]
        Health,
        All_Stats,
        Evasion,

        [JID("bonus_armor")]
        Armor,
        Spell_Amplification,

        [JID("bonus_attack_speed")]
        Attack_Speed,
        Mana_Regen,
        Mana,

        [JID("bonus_intellect")]
        Intelligence,
        Cast_Range,
        Magic_Resistance,
        GPM,
        Gold,
        Strength,
        Attack_Range,
        Agility,
        Health_Regen,
        Spell_Lifesteal,

        [JID("stun_duration")]
        Stun
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
        public string ID { get; set; }
        public JID(string id)
        {
            ID = id;
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
    /// Expected
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
}