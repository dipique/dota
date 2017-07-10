using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using DotA.Model.Enums;

namespace DotA.Model
{
    //Parsing from VPK files - instructions
    //https://puppet-master.net/tutorials/source-engine/extract-content-from-vpk-files/
    public abstract class Parseable
    {
        private const string START_IND = "\t//=================================================================================================================";
        private const int MAX_ID = 255; //dota IDs only go up to 255
        private const char BEHAVIOR_SEP = '|';

        public string Name { get; set; }

        [JID("ID")]
        private int id = -1;
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

        public string ImgName { get; set; }

        public bool Valid { get; set; } = true;

        /// <summary>
        /// Stats provided
        /// </summary>
        public decimal Agility => Effects.Where(e => e.Class == EffectClass.Agility).Sum(e => e.Amount);
        public decimal Strength => Effects.Where(e => e.Class == EffectClass.Strength).Sum(e => e.Amount);
        public decimal Intelligence => Effects.Where(e => e.Class == EffectClass.Intelligence).Sum(e => e.Amount);

        //public Ability ActiveAbility { get; set; } = new Ability();
        public List<Effect> Effects { get; set; } = new List<Effect>();
        public EffectType Type { get; set; }

        [JID("AbilityBahavior")]
        public string AbilityBehavior
        {
            set
            {
                var prefix = typeof(EffectType).GetCustomAttribute<Prefix>().Value;
                var behaviors = value.Split(BEHAVIOR_SEP).Select(s => prefix + s.Trim()).ToArray();
                Type = (EffectType)behaviors.Select(a => (int)Enum.Parse(typeof(EffectType), a)).Sum();
            }
        }
            

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();            
            data.Sections.ForEach(s => ParseSection(retVal, s));
            ApplyEntries(retVal, data); //set header-level entries
            return retVal;
        }

        public static void ApplyEntries<T>(T item, Section s) where T: Parseable
        {
            //get tagged properties
            var taggedProperties = typeof(T).GetProperties()
                                            .Where(p => p.GetCustomAttributes(typeof(JID))
                                                         .Any());

            //start by assigning any attributes available
            foreach (Entry e in s.Entries)
            {
                //get a matching property
                var matchingProp = taggedProperties.FirstOrDefault(p => ((JID)p.GetCustomAttribute(typeof(JID))).IDs.Any(id => id == e.Title));
                if (matchingProp != null)
                {
                    if (matchingProp.PropertyType == typeof(decimal))
                    {
                        decimal val = decimal.TryParse(e.Value, out decimal d) ? d : 0;
                        matchingProp.SetValue(item, val);
                    }
                    else
                    {
                        matchingProp.SetValue(item, e.Value);
                    }
                }
                else
                {
                    //Another possibility is that the tag actually applies the primary effect of the item. It can, however,
                    //be deceptively hard to actually locate that effect.
                }
            }
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

        public static List<T> ParseItems<T>(string[] text) where T : Parseable
        {
            //start by creating a hierarchal structure of all this data, discarding the top 2 levels of the structure
            var data = new Section(typeof(T).Name, text)?.Sections?.FirstOrDefault()?.Sections;
            return data == null ? new List<T>() : data.Select(d => ParseItem<T>(d)).ToList();
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

                //set property to the entry value
                SetEffectProperty(effect, entry);

                //Now, get any entries associated with it
                foreach (var associatedEntry in entries.Where(e => e.AssociatedEffectClass == EffectClass.None)
                                                       .Where(e => entry.ExpectedEntries.Select(ee => ee.name).Contains(e.Title)))
                {
                    associatedEntry.ValueDest = entry.ExpectedEntries.First(ee => ee.name == associatedEntry.Title).dest; //I'm not 100% sure I can do this
                    SetEffectProperty(effect, associatedEntry);
                }

                //Add the entry
                Effects.Add(effect);
            }
        }

        public static void SetEffectProperty(Effect effect, Entry entry)
        {
            //set property to the entry value
            var propName = entry.ValueDest ?? nameof(Effect.Amount);
            var destProp = typeof(Effect).GetProperty(propName);

            //All arrays are numeric but they all represent scaling with levels
            var destPropNumericArray = destProp.PropertyType.IsArray;

            if (destProp.PropertyType.IsArray)
            {
                decimal[] newVal = new decimal[] { entry.NumericValue };
                destProp.SetValue(effect, newVal);
            }
            else
            {
                destProp.SetValue(effect, entry.IsNumericValue ? (object)entry.NumericValue : entry.Value);
            }
        }
    }

    public enum LineType
    {
        None,
        NewItemIndTop,
        ItemTitle,
        NewItemIndBot,
        ItemData,
        EffectDataSection,
        SpecificEffectData,
        OpenItemBrace,
        CloseItemBrace,

    }

    public enum SectionType
    {
        Numeric = 0,
        Master,
        Item,
        ItemRequirements,
        AbilitySpecial
    }

    public class Section
    {
        private const string COMMENT_IND = "//";
        private const string VAR_TYPE_IND = "var_type";

        public string Name { get; set; }
        public SectionType Type { get; set; } = SectionType.Numeric;
        public List<Entry> Entries { get; set; } = new List<Entry>();
        public List<Section> Sections { get; set; } = new List<Section>();

        public Section(string name, string[] data) //, SectionType type = SectionType.Numeric)
        {
            Name = name;
            List<string> buffer = new List<string>();
            string bufferName = string.Empty;
            int currentBracketLevel = 0;
            foreach (string s in data)
            {
                //remove/skip comments
                string line = s.Replace("\"", "").Trim(); //no quotes
                int commentStart = s.IndexOf(COMMENT_IND);
                if (commentStart == 0) continue;
                if (commentStart > 0) line = s.Substring(0, commentStart).Trim();
                if (line.Length == 0) continue;

                //If we're not just buffering a new section, create entries for data encountered
                if (currentBracketLevel == 0)
                {
                    //get non-blank line values
                    string[] values = line.Split('\t')
                                          .Select(l => l.Trim())
                                          .Where(l => !string.IsNullOrEmpty(l))
                                          .ToArray();

                    //create an entry for dual values
                    if (values.Count() == 2)
                    {
                        Entries.Add(new Entry(values[0], values[1]));
                        continue;
                    }
                }

                //deal with brackets
                if (line == "{")
                {
                    if (currentBracketLevel > 0) buffer.Add(line); //keep brackets that are inside sections
                    currentBracketLevel++;
                }
                else if (line == "}")
                {
                    if (currentBracketLevel == 1)
                    {
                        Sections.Add(new Section(bufferName, new List<string>(buffer).ToArray()));
                        buffer.Clear();
                    }
                    else buffer.Add(line);
                    currentBracketLevel--;
                }
                else buffer.Add(line);

                if (currentBracketLevel == 0) bufferName = line;
            }
        }

        //Recursively fetches all entries
        public List<Entry> GetAllEntries(bool ignore_var_type = true)
        {
            var retVal = new List<Entry>();
            retVal.AddRange(Entries);
            Sections.ForEach(s => retVal.AddRange(s.GetAllEntries()));
            return ignore_var_type ? retVal.Where(e => e.Title != VAR_TYPE_IND).ToList() 
                                   : retVal;
        }
    }

    public class Entry
    {
        private const char NUM_SEP = ' '; //this is the character that separates numeric values in scaling attributes

        public string Title { get; private set; }
        public string Value { get; private set; }

        public string ValueDest { get; set; } = null;
        public decimal NumericValue => NumericArray[0];
        public decimal[] NumericArray { get; private set; } = new decimal[] { 0 };
        public bool IsNumericValue { get; set; } = false;
        public bool IsPercentage { get; set; } = false;
        public EffectClass AssociatedEffectClass { get; set; } = EffectClass.None;
        public bool ActiveEffect { get; set; } = false;
        public (string name, string dest)[] ExpectedEntries { get; set; }
        public bool IsDuration { get; set; } = false;

        public Entry(string title, string value)
        {
            Title = title;
            Value = value;
            ReadMeta();
        }

        /// <summary>
        /// Reads the attribute metadata associated with this enum entry
        /// </summary>
        public void ReadMeta()
        {
            //First, check if there an enum about this entry
            var matchingEffect = typeof(EffectClass).GetFields()
                                                    .FirstOrDefault(f => f.GetCustomAttribute<JID>()?.IDs?.Any(id => id == Title) ?? false);
            //and if so, read the metadata
            if (matchingEffect != null)
            {
                AssociatedEffectClass = (EffectClass)Enum.Parse(typeof(EffectClass), matchingEffect.Name, false);

                //get the property where the value should be assigned
                ValueDest = matchingEffect.GetCustomAttribute<ValueDest>()?.DestProperty; 

                IsPercentage = matchingEffect.GetCustomAttribute<PercentEffect>() != null;
                ActiveEffect = matchingEffect.GetCustomAttribute<ActiveEffect>() != null;
                ExpectedEntries = matchingEffect.GetCustomAttributes<ExpectedEntry>().Select(a => (a.Indicator, a.DestField)).ToArray();
            }

            //Parse the value if needed            
            bool isNumeric = true;
            List<decimal> numValues = new List<decimal>();
            
            foreach (string val in Value.Split(NUM_SEP))
            {
                if (decimal.TryParse(val, out decimal d))
                {
                    numValues.Add(IsPercentage ? d / 100 : d);
                }
                else //if we find a single non-numeric value, stop trying to look for one
                {
                    isNumeric = false;
                    break;
                }
            }
            IsNumericValue = isNumeric;
            if (IsNumericValue)
            {
                NumericArray = numValues.ToArray();
            }
        }
    }
}
