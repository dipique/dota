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

        [JID("bonus_agility", "marksmanship_agility_bonus")]
        Agility,

        [JID("bonus_all_stats")]
        All_Stats,

        [JID("bonus_armor")]
        Armor,

        [JID("armor_aura")]
        [ExpectedEntry("aura_radius", nameof(Ability.Radius))]
        Armor_Aura,

        [JID("corruption_armor", "negative_armor")]
        [FlipNegative]
        [ExpectedEntry("curruption_duration", nameof(Effect.Duration))]
        Armor_Corruption,

        [JID("enfeeble_attack_reduction", "armor_reduction")]
        Attack_Reduction,

        [JID("base_attack_range")]
        Attack_Range,

        [JID("bonus_attack_speed")]
        Attack_Speed,

        [JID("attack_speed_per_instance")]
        Attack_Speed_Per_Instance,

        [JID("attack_speed_bonus_pct")]
        [Percentage]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        Attack_Speed_Percent,

        [JID("bash_chance")]
        [ValueDest(nameof(Effect.Chance))]
        [ExpectedEntry("bash_duration", nameof(Effect.Duration))]
        [ExpectedEntry("stun_duration", nameof(Effect.Duration))]
        [ExpectedEntry("bash_cooldown", nameof(Effect.EffectResetTime))]
        [ExpectedEntry("bonus_chance_damage", nameof(Effect.Damage))]
        [ExpectedEntry("bonus_damage", nameof(Effect.Damage))]
        Bash,

        [JID("bash_chance_melee")]
        [ValueDest(nameof(Effect.Chance))]
        [ExpectedEntry("bash_duration", nameof(Effect.Duration))]
        [ExpectedEntry("stun_duration", nameof(Effect.Duration))]
        [ExpectedEntry("bash_cooldown", nameof(Effect.EffectResetTime))]
        [ExpectedEntry("bonus_chance_damage", nameof(Effect.Damage))]
        [ExpectedEntry("bonus_damage", nameof(Effect.Damage))]
        Bash_Melee,

        [JID("bash_change_ranged")]
        [ValueDest(nameof(Effect.Chance))]
        [ExpectedEntry("bash_duration", nameof(Effect.Duration))]
        [ExpectedEntry("bash_cooldown", nameof(Effect.EffectResetTime))]
        [ExpectedEntry("bonus_chance_damage", nameof(Effect.Damage))]
        Bash_Ranged,

        [ValueDest(nameof(Ability.CastRange))]
        [JID("blink_range")]
        Blink,

        [JID("cast_range_bonus")]
        Cast_Range,

        [JID("damage_cleanse")]
        Debuff_On_Damage,

        [JID("bonus_cooldown")]
        Cooldown_Reduction,

        [JID("crit_chance")]
        Crit,

        [ValueDest("BaseDamage")]
        [JID("bonus_damage")]
        Damage,

        [JID("damage_aura")]
        [ExpectedEntry("aura_radius", nameof(Ability.Radius))]
        Damage_Aura,

        [JID("trueshot_ranged_damage")]
        [ValueDest(nameof(Effect.Damage))]
        Damage_Aura_Ranged_Global,

        [JID("bonus_damage_per_instance")]
        Damage_Per_Instance,

        [JID("damage_reduction")]
        [FlipNegative]
        [ExpectedEntry("reduction_duration", nameof(Effect.Duration))]
        Damage_Reduction,

        [JID("sheep_duration")]
        [ValueDest(nameof(Effect.Duration))]
        Disable,

        [ActiveEffect]
        [JID("blast_dot_duration", "tick_rate")] //this actually contains the number of ticks
        [ExpectedEntry("blast_dot_damage", nameof(Effect.Damage))]
        [ExpectedEntry("blast_speed", nameof(Ability.ProjectileSpeed))]
        DOT_Ticks, //distinct from passive DOT like necro

        [JID("bonus_evasion")]
        [Percentage]
        Evasion,

        [ActiveEffect]
        [ValueDest(nameof(Ability.Cooldown))]
        [JID("bonus_gold")]
        [ExpectedEntry("transmute_cast_range_tooltip", "CastRange")]
        Gold,

        [JID("health_bonus")]
        Health,

        [ActiveEffect]
        [JID("health_restore")]
        Health_Regen,

        [JID("health_regen_per_instance")]
        Health_Regen_Per_Instance,

        [JID("health_restore")]
        Health_Restore,

        [JID("bonus_intellect", "bonus_intelligence")]
        Intelligence,

        [ActiveEffect]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        [JID("fade_time", "fade_delay")]
        Invisibility,

        [ActiveEffect]
        [JID("knockback_distance_max", "knockback_distance", "knockback_max", "travel_distance")]
        [ExpectedEntry("knockback_duration", nameof(Effect.Duration))]
        Knockback,

        [Percentage]
        [JID("lifesteal_percent")]
        Lifesteal,

        [Percentage]
        [JID("vampiric_aura")]
        [ExpectedEntry("vampiric_aura_radius", nameof(Ability.Radius))]
        [ExpectedEntry("aura_radius", nameof(Ability.Radius))]
        Lifesteal_Aura,

        [JID("bonus_magical_armor")]
        Magic_Resistance,

        [JID("bonus_mana")]
        Mana,

        [JID("mana_per_hit")]
        Mana_Burn,

        [JID("mana_restore")]
        Mana_Regen,

        [JID("mana_restore")]
        Mana_Restore,

        [JID("bonus_movement")]
        Movement_Speed,

        [Percentage]
        [JID("movement_speed_percent_bonus")]
        Movement_Speed_Pct,

        [JID("move_speed_per_instance")]
        Movement_Speed_Per_Instance,
        
        [JID("silence_radius")]
        [ValueDest(nameof(Ability.Radius))]
        [AltJID("silence_duration", nameof(Effect.Duration))]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        Silence,

        [JID("slow", "frost_arrows_movement_speed")]
        [FlipNegative]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        Slow,

        [Percentage]
        [JID("spell_amp")]
        Spell_Amplification,

        [JID("bonus_strength")]
        Strength,

        [Percentage]
        Spell_Lifesteal,

        [ActiveEffect]
        [JID("stun_duration")]
        [ValueDest(nameof(Effect.Duration))]
        Stun,

        [ActiveEffect]
        [JID("blast_stun_duration", "bolt_stun_duration")]
        [ValueDest(nameof(Effect.Duration))]
        Stun_Projectile,

        [ActiveEffect]
        [JID("extra_spell_damage_precent")]
        [Percentage]
        Take_Extra_Magic_Damage,

        [JID("true_sight_range")]
        True_Sight,

        [JID("vision_radius")]
        [ValueDest(nameof(Ability.Radius))]
        Vision,

        [JID("xp_bonus")]
        XP_Gain
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