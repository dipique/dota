using System.Collections.Generic;
using System.Linq;

using DotA.Model.Attributes;
using DotA.Model.Enums;

namespace DotA.Model
{
    [ImageFolder("item")]
    [Prefix("item_")]
    public class Item : Parseable
    {
        private const string RECIPE_IND = "item_recipe_";
        private const char RECIPE_SEP = ';';
        private const string ABILITY_NAME_SUFFIX = "_ability";

        public virtual decimal Agility(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Agility, EffectClass.All_Stats }, lvl);
        public virtual decimal Strength(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Strength, EffectClass.All_Stats }, lvl);
        public virtual decimal Intelligence(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Intelligence, EffectClass.All_Stats }, lvl);

        public decimal GetAmountByClass(EffectClass[] classes, int lvl = 1) => Ability.Effects.Where(e => classes.Any(c => c == e.Class))
                                                                                              .Sum(e => e.Amount[lvl - 1]);

        public Ability Ability { get; set; } = new Ability();

        public override void ApplyHeaderLevelEntries(List<Entry> entries) => entries.ForEach(e => e.Apply(this, Ability, Ability.ActiveEffect));

        public bool IsRecipe => (Recipe?.Count() ?? 0) > 0;

        /// <summary>
        /// The notion of having an ability in the item doesn't really exist in the source files, so we need to give the ability a name
        /// </summary>
        public override string Name
        {
            get => base.Name;
            set
            {
                base.Name = value;
                Ability.Name = $"{value}{ABILITY_NAME_SUFFIX}";
            }
        }

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
                    Class = entry.AssociatedEffectClass,
                    ParentName = Name
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
