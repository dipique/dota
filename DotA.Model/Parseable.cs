using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using DotA.Model.Attributes;
using DotA.Model.Extensions;
using DotA.Model.Enums;

namespace DotA.Model
{
    //Parsing from VPK files - instructions
    //https://puppet-master.net/tutorials/source-engine/extract-content-from-vpk-files/
    public abstract class Parseable
    {
        [IgnoreDataMember]
        [NoDisplay]
        public virtual int MAX_ID => 255; //dota IDs only go up to 255
        private const char BEHAVIOR_SEP = '|';

        [NoDisplay]
        [IgnoreDataMember]
        public virtual bool RequiresID => true;

        [DisplayOnly]
        [FieldOrder(1)]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                {
                    if (string.IsNullOrEmpty(Name)) return string.Empty;
                    string prefix = GetType().GetCustomAttribute<Prefix>()?.Value;
                    string working = string.IsNullOrEmpty(prefix) ? Name : Name.Replace(prefix, string.Empty);
                    var words = working.Split(new char[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s => s.Trim().ToLower())
                                       .Select(s => s[0].ToString().ToUpper() + (s.Length > 1 ? s.Substring(1) : string.Empty));
                    displayName = string.Join(" ", words);
                }
                return displayName;
            }
            set => displayName = value;
        }
        private string displayName = string.Empty;

        [FieldOrder(2)]
        [PrimaryKey]
        public virtual string Name { get; set; }

        [FieldOrder(0)]
        [JID("ID","HeroID")]
        public string ID
        {
            get => id.ToString();
            set => id = int.TryParse(value, out int i) ? i : -1;
        }
        private int id = -1;

        private string imgName = string.Empty;
        public string ImgName
        {
            get
            {
                if (string.IsNullOrEmpty(imgName))
                {
                    string prefix = GetType().GetCustomAttribute<Prefix>()?.Value;
                    string working = string.IsNullOrEmpty(prefix) ? Name : Name.Replace(prefix, string.Empty);
                    imgName = $"120px-{working}_icon.png";
                }
                return imgName;
            }
            set => imgName = value;
        }

        /// <summary>
        /// Gets an item by name whether it's this item, an item that is one of its properties, or an item contained in a
        /// list. As long as it's parseable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Parseable GetItemByName(string name)
        {
            if (Name == name) return this; //hey it's me!

            //see if it's in a property that is a list
            return GetType().GetProperties()
                            .Where(p => p.PropertyType.IsList())
                            .Where(p => p.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(Parseable)))
                            .SelectMany(p => p.GetListValue<Parseable>(this))
                            .Cast<Parseable>()
                            .FirstOrDefault(prop => prop.Name == name) ??
                   //if not, try a normal property
                   GetType().GetProperties()  
                            .Where(p => p.PropertyType.IsSubclassOf(typeof(Parseable)))
                            .Select(p => p.GetValue<Parseable>(this))
                            .FirstOrDefault(prop => prop.Name == name);
        }

        [NoDisplay]
        public virtual bool Valid => !RequiresID || (id > 0 && id < MAX_ID);

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();
            retVal.Name = data.Name;
            data.Sections.ForEach(s => ParseSection(retVal, s));

            //apply all the top level entries. We do this after the section application because sometimes these entries
            //need to be applied to objects effects created by those sections.
            retVal.ApplyHeaderLevelEntries(data.Entries.Where(e => !e.IsNumericValue || e.NumericValue != 0).ToList());

            return retVal;
        }

        public virtual void ApplyHeaderLevelEntries(List<Entry> entries)
        {
            entries.ForEach(e => e.Apply(this));
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

        /// <summary>
        /// Here's how this assigns effects:
        /// 
        /// 1. Loop through all entries that have a uniquely assigned effectclass via JID (must be at least one or
        ///    it wouldn't know what effect there was)
        /// 2. Takes entries that are listed as expected entries and assign them
        /// 3. Ignores unassigned entries
        /// </summary>
        public void ParseAbilitySpecial(Section s, List<Effect> effectList, params object[] applicationOrder)
        {
            //For testing, so we can see how things are applying
            //if (Name.Contains("wave_of"))
            //{ }

            var entries = s.GetAllEntries();

            //Every entry with a JID will have it own effect.
            foreach (var entry in entries.Where(e => e.AssociatedEffectClass != EffectClass.None))
            {
                var effect = new Effect()
                {
                    Class = entry.AssociatedEffectClass,
                    ParentName = Name, 
                    IsPassive = !entry.ActiveEffect
                };

                //set property to the entry value--effect first, then any other items as specified
                entry.Apply(effect, applicationOrder);

                //Now, get any entries associated with it
                foreach (var associatedEntry in entries.Where(e => e.AssociatedEffectClass == EffectClass.None)
                                                       .Where(e => entry.ExpectedEntries.Select(ee => ee.name).Contains(e.Title)))
                {
                    associatedEntry.ValueDest = entry.ExpectedEntries.First(ee => ee.name == associatedEntry.Title).dest;
                    associatedEntry.Apply(effect, this);
                }

                //Add the entry
                effectList.Add(effect);
            }
        }
    }
}
