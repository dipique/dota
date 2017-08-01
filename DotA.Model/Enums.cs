using System;
using DotA.Model.Attributes;

namespace DotA.Model.Enums
{
    public enum DisableType
    {
        None,
        Root,
        Stun
    }

    [Prefix("DAMAGE_TYPE_")]
    public enum DamageType
    {
        NONE,
        PHYSICAL,
        MAGICAL,
        PURE
    }

    [Prefix("SPELL_IMMUNITY_")]
    public enum SpellImmunityPiercingType
    {
        ENEMIES_NO,
        ENEMIES_YES,
        ALLIES_NO,
        ALLIES_YES
    }

    [Prefix("SPELL_DISPELLABLE_")]
    public enum DispellableType
    {
        None,
        YES,
        YES_STRONG,
        NO
        
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

    [Prefix("DOTA_ATTRIBUTE_")]
    public enum StatType
    {
        STRENGTH,
        AGILITY,
        INTELLECT
    }

    [Prefix("DOTA_UNIT_CAP_")]
    public enum AttackType
    {
        MELEE_ATTACK,
        RANGED_ATTACK,
        NO_ATTACK
    }

    public enum EffectClass
    {
        None = 0,

        [JID("corruption_armor", "negative_armor")]
        [FlipNegative]
        [ExpectedEntry("curruption_duration", nameof(Effect.Duration))]
        Armor_Corruption,

        [JID("attack_speed_bonus_pct")]
        [Percentage]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        Attack_Speed_Percent,

        [JID("bash_change_melee")]
        [ValueDest(nameof(Effect.Chance))]
        [ExpectedEntry("bash_duration", nameof(Effect.Duration))]
        [ExpectedEntry("bash_cooldown", nameof(Effect.EffectResetTime))]
        [ExpectedEntry("bonus_chance_damage", nameof(Effect.BaseDamage))]
        Bash_Melee,

        [JID("bash_change_ranged")]
        [ValueDest(nameof(Effect.Chance))]
        [ExpectedEntry("bash_duration", nameof(Effect.Duration))]
        [ExpectedEntry("bash_cooldown", nameof(Effect.EffectResetTime))]
        [ExpectedEntry("bonus_chance_damage", nameof(Effect.BaseDamage))]
        Bash_Ranged,

        [ValueDest(nameof(Ability.CastRange))]
        [JID("blink_range")]
        Blink,

        [JID("crit_chance")]
        Crit,

        [ValueDest("BaseDamage")]
        [JID("bonus_damage")]
        Damage,

        [JID("sheep_duration")]
        [ValueDest(nameof(Effect.Duration))]
        Disable,

        [Percentage]
        [JID("lifesteal_percent")]
        Lifesteal,

        [JID("mana_per_hit")]
        Mana_Burn,

        [ValueDest()]
        [Percentage]
        [JID("movement_speed_percent_bonus")]
        Movement_Speed_Pct,

        [JID("slow")]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        Slow,

        [JID("xp_bonus")]
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
        [Percentage]
        Evasion,

        [JID("bonus_armor")]
        Armor,

        [Percentage]
        [JID("spell_amp")]
        Spell_Amplification,

        [JID("bonus_attack_speed")]
        Attack_Speed,

        [JID("mana_restore")]
        Mana_Regen,

        [JID("bonus_mana")]
        Mana,

        [JID("bonus_intellect", "bonus_intelligence")]
        Intelligence,

        [JID("cast_range_bonus")]
        Cast_Range,

        [JID("bonus_magical_armor")]
        Magic_Resistance,

        [ActiveEffect]
        [ValueDest(nameof(Ability.Cooldown))]
        [JID("bonus_gold")]
        [ExpectedEntry("transmute_cast_range_tooltip", "CastRange")]
        Gold,

        [JID("bonus_strength")]
        Strength,

        [JID("base_attack_range")]
        Attack_Range,

        [JID("bonus_agility")]
        Agility,

        [ActiveEffect]
        [JID("health_restore")]
        Health_Regen,

        [Percentage]
        Spell_Lifesteal,

        [ActiveEffect]
        [JID("extra_spell_damage_precent")]
        [Percentage]
        Take_Extra_Magic_Damage,

        [ActiveEffect]
        [JID("stun_duration")]
        [ValueDest(nameof(Effect.Duration))]
        Stun,

        [JID("true_sight_range")]
        True_Sight,

        [ActiveEffect]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        [JID("fade_time", "fade_delay")]
        Invisibility
    }

    [Prefix("DOTA_ABILITY_BEHAVIOR_")]
    [Flags]
    public enum EffectType //src: items.txt file
    {
        NONE = 0,

        HIDDEN = 1,

        PASSIVE = 1 << 1,

        NO_TARGET = 1 << 2,

        [ActiveEffect]
        UNIT_TARGET = 1 << 3,

        [ActiveEffect]
        POINT = 1 << 4,

        AOE = 1 << 5,

        NOT_LEARNABLE = 1 << 6,

        [ActiveEffect]
        CHANNELLED = 1 << 7,

        ITEM = 1 << 8,

        [ActiveEffect]
        TOGGLE = 1 << 9,

        DIRECTIONAL = 1 << 10,

        IMMEDIATE = 1 << 11,

        [ActiveEffect]
        ROOT_DISABLES = 1 << 12,

        DONT_RESUME_ATTACK = 1 << 13,

        [ActiveEffect]
        OPTIONAL_UNIT_TARGET = 1 << 14,

        NOASSIST = 1 << 15,

        DONT_CANCEL_CHANNEL = 1 << 16,

        IGNORE_CHANNEL = 1 << 17,

        DONT_CANCEL_MOVEMENT = 1 << 18,

        DONT_RESUME_MOVEMENT = 1 << 19,

        UNRESTRICTED = 1 << 20,

        IGNORE_BACKSWING = 1 << 21,

        IGNORE_PSEUDO_QUEUE = 1 << 22,

        [ActiveEffect]
        AUTOCAST = 1 << 23,

        ATTACK = 1 << 24,

        AURA = 1 << 27,

        NORMAL_WHEN_STOLEN = 1 << 26,

        RUNE_TARGET = 1 << 27,

        DONT_ALERT_TARGET = 1 << 28,

        DOTA_ABILITY_TYPE_ULTIMATE = 1 << 29 //this randomly shows up in lone druid's battle cry
    }

    [Prefix("DOTA_ABILITY_TYPE_")]
    public enum AbilityType
    {
        BASIC,
        ULTIMATE,
        ATTRIBUTES
    }

    [Prefix("DOTA_UNIT_TARGET_TEAM_")]
    [Flags]
    public enum TargetTeam
    { 
        NONE = 0,
        FRIENDLY_HERO = 1,
        FRIENDLY_BASIC = 1 << 1,
        FRIENDLY = 1 << 2,
        ENEMY_HERO = 1 << 3,
        ENEMY_BASIC = 1 << 4,
        ENEMY = 1 << 5,
        ALL = 1 << 6,
        BOTH = 1 << 7,
        CUSTOM = 1 << 8
    }

    [Prefix("DOTA_UNIT_TARGET_")]
    [Flags]
    public enum TargetType
    {
        NONE = 0,
        BASIC = 1,
        HERO = 1 << 1,
        CREEP = 1 << 2,
        CUSTOM = 1 << 3,
        BUILDING = 1 << 4,
        TREE = 1 << 5
    }

    public enum LineType
    {
        None,
        NewItemIndTop,
        ItemTitle,
        NewItemIndBot,
        ItemData,
        EffectDataSection,
        SpecificEffectData,
        OpenItemBrace,
        CloseItemBrace,

    }

    public enum SectionType
    {
        Numeric = 0,
        Master,
        Item,
        ItemRequirements,
        AbilitySpecial
    }
}