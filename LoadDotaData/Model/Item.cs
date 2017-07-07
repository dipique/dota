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
        private const string RECIPE_IND = "item_recipe_";
        private const char RECIPE_SEP = ';';

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
        
        public decimal FullItemCost => Recipe.Count() > 0 ? GetRecipeItems.Sum(i => i.ItemCost) + ItemCost : ItemCost;

        [JID("ItemCost")]
        public decimal ItemCost { get; set; }

        /// <summary>
        /// Items that produce 
        /// </summary>
        public string[] Recipe { get; set; }

        public List<Item> GetRecipeItems { get; set; } //TODO this should do a lookup to get all the items from the strings

        [SpecialHandlerSectionMethod("ItemRequirements")]
        public void AddRecipe(Section s)
        {
            if (s.Entries.Count == 0) return;
            IsRecipe = true;
            Recipe = s.Entries.First().Value.Split(RECIPE_SEP);
        }

    }
}
