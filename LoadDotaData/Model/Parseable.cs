using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    //Parsing from VPK files - instructions
    //https://puppet-master.net/tutorials/source-engine/extract-content-from-vpk-files/
    public abstract class Parseable
    {
        private const int MAX_ID = 255; //dota IDs only go up to 255
        private const char BEHAVIOR_SEP = '|';

        public virtual string Name { get; set; }       
        
        [JID("ID", "HeroID")]
        public string ID
        {
            get => id.ToString();
            set
            {
                if (int.TryParse(value, out int i))
                {
                    id = i;
                    Valid = i < 0 || i > MAX_ID;
                }
            }
        }
        private int id = -1;

        public string ImgName { get; set; }

        public bool Valid { get; set; } = true;

        /// <summary>
        /// Stats provided
        /// </summary>
        public virtual decimal Agility(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Agility, EffectClass.All_Stats }, lvl);
        public virtual decimal Strength(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Strength, EffectClass.All_Stats }, lvl);
        public virtual decimal Intelligence(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Intelligence, EffectClass.All_Stats }, lvl);

        public decimal GetAmountByClass(EffectClass[] classes, int lvl = 1) => Effects.Where(e => classes.Any(c => c == e.Class))
                                                                                      .Sum(e => e.Amount[lvl - 1]);

        [JID("AbilityCastRange")]
        public decimal[] CastRange { get; set; } = new decimal[] { 0 };

        [JID("AbilityCastPoint")]
        public decimal CastPoint { get; set; } = 0;

        [JID("AbilityCooldown")]
        public decimal[] Cooldown { get; set; } = new decimal[] { 0 };

        [JID("AbilityManaCost")]
        public decimal[] ManaCost { get; set; } = new decimal[] { 0 };

        public List<Effect> Effects { get; set; } = new List<Effect>();
        public EffectType Type { get; set; }
        public Effect ActiveEffect
        {
            get
            {
                var activeEffects = Effects.Where(e => e.IsActive);
                if (activeEffects.Count() == 1) return activeEffects.First();
                if (activeEffects.Count() == 0) return null;

                //TODO: stiffen up these parameters
                return activeEffects.FirstOrDefault();
            }
        }

        [JID("AbilityBehavior")]
        public string AbilityBehavior
        {
            set
            {
                var prefix = typeof(EffectType).GetCustomAttribute<Prefix>().Value;
                var behaviors = value.Split(BEHAVIOR_SEP).Select(s => s.Trim().Replace(prefix, string.Empty)).ToArray();
                Type = (EffectType)behaviors.Select(a => (int)Enum.Parse(typeof(EffectType), a)).Sum();
            }
        }
            

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();
            retVal.Name = data.Name;
            data.Sections.ForEach(s => ParseSection(retVal, s));

            //apply all the top level entries. We do this after the section application because sometimes these entries
            //need to be applied to effects created by those sections.
            foreach (var entry in data.Entries.Where(e => !e.IsNumericValue || e.NumericValue != 0))
                entry.Apply(retVal, retVal.ActiveEffect); //apply to the active effect if the item doesn't have a matching parameter
            return retVal;
        }

        public static void ParseSection<T>(T item, Section s) where T: Parseable
        {
            //Check if the item has a special handler for this type of section
            var specialParser = typeof(T).GetMethods()
                                         .FirstOrDefault(m => m.GetCustomAttributes<SpecialHandlerSectionMethod>(true) //TODO: prioritize method found in inherited class
                                                               .FirstOrDefault()
                                                              ?.SectionType == s.Name);
            if (specialParser != null)
            {
                specialParser.Invoke(item, new object[] { s });
                return;
            }

            //we don't do anything if we don't know what the section does
        }

        public static List<T> ParseItems<T>(string[] text) where T : Parseable => 
            new Section(typeof(T).Name, text)?.Sections?.FirstOrDefault() //discard the first section
                                             ?.Sections
                                             ?.Select(d => ParseItem<T>(d))
                                             ?.ToList();

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
                entry.Apply(effect, this);

                //Now, get any entries associated with it
                foreach (var associatedEntry in entries.Where(e => e.AssociatedEffectClass == EffectClass.None)
                                                       .Where(e => entry.ExpectedEntries.Select(ee => ee.name).Contains(e.Title)))
                {
                    associatedEntry.ValueDest = entry.ExpectedEntries.First(ee => ee.name == associatedEntry.Title).dest;
                    entry.Apply(effect, this);
                }

                //Add the entry
                Effects.Add(effect);
            }
        }
    }
}
