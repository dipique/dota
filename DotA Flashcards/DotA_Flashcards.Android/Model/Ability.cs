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

namespace DotA_Flashcards.Droid.Model
{
    public class Ability
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal CastAnimation { get; set; }
        public decimal CastAnimationFollowThrough { get; set; }

        public bool AOE { get; set; }
        public bool SelfTargetable { get; set; }
        public bool EnemyTargetable { get; set; }
        public bool AlliesTargetable { get; set; }
        public bool CreepTargetable { get; set; }
        public bool NeutralTargetable { get; set; }

        public decimal[] CastRange { get; set; }
        public decimal[] BaseDamage { get; set; }

        public ScalingType ScalingType { get; set; } = ScalingType.None;

        public int[] Cooldown { get; set; }
        public int MaxLevels { get; set; } //how many ability points can be placed in this ability?

        public bool IsUltimate { get; set; } = false;

        public DisableType DisableType { get; set; } = DisableType.None;
        public decimal DisableDuration { get; set; } = 0m;
        public bool DisjointsProjectiles { get; set; } = false;
        public DamageType DamageType { get; set; } = DamageType.None;

        public bool PiercesBKB { get; set; } = false;
        public LinkensInteraction LinkensInteraction { get; set; } = LinkensInteraction.None;


    }
}