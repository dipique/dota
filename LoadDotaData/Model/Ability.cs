using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;

namespace DotA.Model
{
    public class Ability
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        
        public int MaxLevels { get; set; } //how many ability points can be placed in this ability?

        public bool IsUltimate { get; set; } = false;        

        public List<Effect> Effects { get; set; } = new List<Effect>();
    }
}