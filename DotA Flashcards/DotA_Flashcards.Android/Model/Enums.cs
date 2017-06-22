using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DotA_Flashcards.Droid.Model.Enums
{
    public enum ScalingType
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
        Damage,
        Lifesteal,
        Mobility,
        Slow,
        Disable
    }

    public enum EffectType
    {
        Active,
        Passive
    }
}