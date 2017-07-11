﻿using System;
using DotA.Model.Attributes;

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
        None = 0,

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

        [ValueDest(nameof(Parseable.CastRange))]
        [JID("blink_range")]
        Blink,

        [JID("crit_chance")]
        Crit,

        [ValueDest("BaseDamage")]
        [JID("bonus_damage")]
        Damage,

        [JID("lifesteal_percent")]
        Lifesteal,

        [ValueDest()]
        [JID("movement_speed_percent_bonus")]
        Movement_Speed_Pct,

        [JID("slow")]
        [ExpectedEntry("duration", nameof(Effect.Duration))]
        Slow,

        [JID("sheep_duration")]
        [ValueDest(nameof(Effect.Duration))]
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

        GPM,

        [ActiveEffect]
        [ValueDest(nameof(Parseable.Cooldown))]
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

        [PercentEffect]
        Spell_Lifesteal,

        [ActiveEffect]
        [JID("extra_spell_damage_precent")]
        [PercentEffect]
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
        Invisibility,

        [JID("corruption_armor")]
        [ExpectedEntry("curruption_duration", nameof(Effect.Duration))]
        Armor_Reduction
    }

    [Prefix("DOTA_ABILITY_BEHAVIOR_")]
    [Flags]
    public enum EffectType //src: items.txt file
    {
        HIDDEN = 1,
        PASSIVE = 1 << 1,
        NO_TARGET = 1 << 2,
        UNIT_TARGET = 1 << 3,
        POINT = 1 << 4,
        AOE = 1 << 5,
        NOT_LEARNABLE = 1 << 6,
        CHANNELLED = 1 << 7,
        ITEM = 1 << 8,
        TOGGLE = 1 << 9,
        DIRECTIONAL = 1 << 10,
        IMMEDIATE = 1 << 11,
        ROOT_DISABLES = 1 << 12
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