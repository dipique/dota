using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    [ImageFolder("ability")]
    public class Ability : Parseable
    {
        public string Description { get; set; }

        public int MaxLevels { get; set; } = 1; //how many ability points can be placed in this ability? //TODO: Should this just be read from attributes?

        public bool IsUltimate { get; set; } = false;
    }
}