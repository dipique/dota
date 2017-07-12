using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
    //Parsing from VPK files - instructions
    //https://puppet-master.net/tutorials/source-engine/extract-content-from-vpk-files/
    public abstract class Parseable
    {
        private const string START_IND = "\t//=================================================================================================================";
        private const int MAX_ID = 255; //dota IDs only go up to 255
        private const char BEHAVIOR_SEP = '|';

        public virtual string Name { get; set; }       
        
        [JID("ID")]
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
        public decimal Agility(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Agility, EffectClass.All_Stats }, lvl);
        public decimal Strength(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Strength, EffectClass.All_Stats }, lvl);
        public decimal Intelligence(int lvl = 1) => GetAmountByClass(new EffectClass[] { EffectClass.Intelligence, EffectClass.All_Stats }, lvl);

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
            retVal.Name = data.Name;
            data.Sections.ForEach(s => ParseSection(retVal, s));
            data.Entries.ForEach(e => e.Apply(retVal, retVal.ActiveEffect)); //apply to the active effect if the item doesn't have a matching parameter
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

        private string title = null;
        public string Title
        {
            get => title;
            private set
            {
                title = value;

                //Check if there an enum about this entry title
                var matchingEffect = typeof(EffectClass).GetFields()
                                                        .FirstOrDefault(f => f.GetCustomAttribute<JID>()?.IDs?.Any(id => id == Title) ?? false);
                //and if so, read the metadata
                if (matchingEffect != null)
                {
                    AssociatedEffectClass = (EffectClass)Enum.Parse(typeof(EffectClass), matchingEffect.Name, false);
                    IsPercentage = matchingEffect.GetCustomAttribute<PercentEffect>() != null;
                    ActiveEffect = matchingEffect.GetCustomAttribute<ActiveEffect>() != null;
                    ExpectedEntries = matchingEffect.GetCustomAttributes<ExpectedEntry>().Select(a => (a.Indicator, a.DestField)).ToArray();
                    ValueDest = matchingEffect.GetCustomAttribute<ValueDest>()?.DestProperty;
                }
            }
        }
        private string entryValue = null;
        public string Value
        {
            get => entryValue;
            private set
            {
                entryValue = value;
                //Parse the value if needed            
                bool isNumeric = true;
                List<decimal> numValues = new List<decimal>();

                foreach (string val in value.Split(NUM_SEP))
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

                if (IsNumericValue = isNumeric)
                    NumericArray = numValues.ToArray();
            }
        }

        public string ValueDest { get; set; }
        public decimal NumericValue => NumericArray[0];
        public decimal[] NumericArray { get; private set; } = new decimal[] { 0 };
        public bool IsNumericValue { get; private set; } = false;
        public bool IsPercentage { get; set; } = false;
        public EffectClass AssociatedEffectClass { get; set; } = EffectClass.None;
        public bool ActiveEffect { get; set; } = false;
        public (string name, string dest)[] ExpectedEntries { get; set; }
        public bool IsDuration { get; set; } = false;

        public Entry(string title, string value)
        {
            Title = title;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objCandidates">A list of candidates for entry application in order of precedence</param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool Apply(params object[] objCandidates)
        {
            var validCandidates = objCandidates.Where(o => o != null).ToArray();
            if (validCandidates?.Count() < 1) return false;
            int ind = -1;
            while (true)
            {
                if (++ind == validCandidates.Count()) break; //this is the loop end control             
                object obj = validCandidates[ind];

                //set property to the entry value, then a matching property value, then finally a default if available
                var propName = ValueDest ??
                               obj.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttribute<JID>()?.IDs?.Contains(Title) ?? false)?.Name ??
                               obj.GetType().GetCustomAttribute<DefaultEntryProperty>()?.PropertyName;
                if (string.IsNullOrEmpty(propName)) continue;

                try
                {
                    //Set the property
                    var destProp = obj.GetType().GetProperty(propName);
                    if (destProp == null) continue;
                    if (destProp.PropertyType.IsArray) //All arrays are numeric and represent scaling with levels
                        destProp.SetValue(obj, NumericArray);
                    else
                    {
                        //use the property type to determine how to assign the value
                        if (destProp.PropertyType == typeof(decimal))
                        {
                            destProp.SetValue(obj, NumericValue);
                        }
                        else
                        {
                            destProp.SetValue(obj, Value);
                        }
                    }

                    return true;
                }
                catch { continue; }
            }

            //if we're here, it means we never actually found a valid object
            return false;
        }
    }
}
