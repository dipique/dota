using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;

namespace DotA.Model
{
    [ImageFolder("item")]
    public class Item
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string ImgName { get; set; }
        public bool IsRecipe { get; set; }

        /// <summary>
        /// Stats provided by item
        /// </summary>
        public decimal Agility { get; set; }
        public decimal Strength { get; set; }
        public decimal Intelligence { get; set; }

        private decimal cost = 0; 
        public decimal ItemCost
        {
            get => Recipe.Count > 0 ? Recipe.Sum(i => i.ItemCost) : cost;
            set => cost = value;
        }

        /// <summary>
        /// Items that produce 
        /// </summary>
        public List<Item> Recipe { get; set; }

        public List<Effect> Effects { get; set; } = new List<Effect>();

    }
}
