using System.Collections.Generic;
using System.Linq;

using DotA.Model.Attributes;
using DotA.Model.Enums;

namespace DotA.Model
{
    [ImageFolder("item")]
    public class Item : Parseable
    {
        private const string RECIPE_IND = "item_recipe_";
        private const char RECIPE_SEP = ';';

        public virtual decimal Agility(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Agility, EffectClass.All_Stats }, lvl);
        public virtual decimal Strength(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Strength, EffectClass.All_Stats }, lvl);
        public virtual decimal Intelligence(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Intelligence, EffectClass.All_Stats }, lvl);

        public decimal GetAmountByClass(EffectClass[] classes, int lvl = 1) => Active.Effects.Where(e => classes.Any(c => c == e.Class))
                                                                                              .Sum(e => e.Amount[lvl - 1]);

        public Ability Active { get; set; } = new Ability();
        public List<Effect> Passives { get; set; } = new List<Effect>(); //TODO: Save passives in a separate list

        public override void ApplyHeaderLevelEntries(List<Entry> entries) => entries.ForEach(e => e.Apply(this, Active, Active.ActiveEffect));

        public bool IsRecipe => (Recipe?.Count() ?? 0) > 0;
        
        /// <summary>
        /// Not yet fully implemented because other than actual recipe items, the component items aren't present in the files we're parsing
        /// </summary>
        public decimal FullItemCost(List<Item> allItems) => Recipe.Count() > 0 ? GetRecipeItems(allItems).Sum(i => i.ItemCost) + ItemCost 
                                                                               : ItemCost;

        [JID("ItemCost")]
        public decimal ItemCost { get; set; }

        /// <summary>
        /// Items that produce 
        /// </summary>
        public string[] Recipe { get; set; }

        public List<Item> GetRecipeItems(List<Item> allItems) => allItems.Where(a => Recipe.Any(r => r == a.Name)).ToList();

        [SpecialHandlerSectionMethod("ItemRequirements")]
        public void AddRecipe(Section s)
        {
            if (s.Entries.Count == 0) return;
            Recipe = s.Entries.First().Value.Split(RECIPE_SEP);
        }

        [SpecialHandlerSectionMethod("AbilitySpecial")]
        public void ParseAbilitySpecial(Section s)
        {
            var entries = s.GetAllEntries();

            //Every entry with a JID will have it own effect.
            foreach (var entry in entries.Where(e => e.AssociatedEffectClass != EffectClass.None))
            {
                var effect = new Effect() {
                    Class = entry.AssociatedEffectClass
                };

                //set property to the entry value--effect frist, then the base item
                entry.Apply(effect, Active, this);

                //Now, get any entries associated with it
                foreach (var associatedEntry in entries.Where(e => e.AssociatedEffectClass == EffectClass.None)
                                                       .Where(e => entry.ExpectedEntries.Select(ee => ee.name).Contains(e.Title)))
                {
                    associatedEntry.ValueDest = entry.ExpectedEntries.First(ee => ee.name == associatedEntry.Title).dest;
                    entry.Apply(effect, Active, this);
                }

                //Add the entry
                Active.Effects.Add(effect);
            }
        }

    }
}
