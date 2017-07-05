using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Enums;

namespace DotA.Model
{
    [ImageFolder("item")]
    public class Item : Parseable
    {
        private const string RECIPE_IND = "Recipe:";

        public Item() { }

        private string name = string.Empty;
        public new string Name
        {
            get => name;
            set
            {
                IsRecipe = value.StartsWith(RECIPE_IND);
                name = value;
            }
        }


        public bool IsRecipe { get; set; }

        private decimal cost = 0; 
        [JID("ItemCost")]
        public decimal ItemCost
        {
            get => Recipe.Count > 0 ? GetRecipeItems.Sum(i => i.ItemCost) : cost;
            set => cost = value;
        }

        /// <summary>
        /// Items that produce 
        /// </summary>
        public List<string> Recipe { get; set; }

        public List<Item> GetRecipeItems { get; set; } //TODO this should do a lookup to get all the items from the strings

    }
}
