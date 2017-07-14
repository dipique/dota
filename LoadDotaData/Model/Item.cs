using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotA.Model.Attributes;
using DotA.Model.Enums;

namespace DotA.Model
{
    [ImageFolder("item")]
    public class Item : Parseable
    {
        private const string RECIPE_IND = "item_recipe_";
        private const char RECIPE_SEP = ';';

        /// <summary>
        /// Stats provided
        /// </summary>
        public virtual decimal Agility(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Agility, EffectClass.All_Stats }, lvl);
        public virtual decimal Strength(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Strength, EffectClass.All_Stats }, lvl);
        public virtual decimal Intelligence(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Intelligence, EffectClass.All_Stats }, lvl);

        public decimal GetAmountByClass(EffectClass[] classes, int lvl = 1) => Ability.Effects.Where(e => classes.Any(c => c == e.Class))
                                                                                              .Sum(e => e.Amount[lvl - 1]);

        public Ability Ability { get; set; } = new Ability();

        public override void ApplyHeaderLevelEntries(List<Entry> entries) => entries.ForEach(e => e.Apply(this, Ability, Ability.ActiveEffect));

        public Item() { }

        private string name = string.Empty;
        public override string Name
        {
            get => name;
            set
            {
                IsRecipe = value.StartsWith(RECIPE_IND);
                name = value;
            }
        }

        public bool IsRecipe { get; set; }
        
        public decimal FullItemCost => Recipe.Count() > 0 ? GetRecipeItems.Sum(i => i.ItemCost) + ItemCost 
                                                          : ItemCost;

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

        [SpecialHandlerSectionMethod("AbilitySpecial")]
        public void ParseAbilitySpecial(Section s)
        {
            var entries = s.GetAllEntries();

            //Every entry with a JID will have it own effect.
            foreach (var entry in entries.Where(e => e.AssociatedEffectClass != EffectClass.None))
            {
                var effect = new Effect()
                {
                    Class = entry.AssociatedEffectClass
                };

                //set property to the entry value--effect frist, then the base item
                entry.Apply(effect, Ability, this);

                //Now, get any entries associated with it
                foreach (var associatedEntry in entries.Where(e => e.AssociatedEffectClass == EffectClass.None)
                                                       .Where(e => entry.ExpectedEntries.Select(ee => ee.name).Contains(e.Title)))
                {
                    associatedEntry.ValueDest = entry.ExpectedEntries.First(ee => ee.name == associatedEntry.Title).dest;
                    entry.Apply(effect, Ability, this);
                }

                //Add the entry
                Ability.Effects.Add(effect);
            }
        }

    }
}
