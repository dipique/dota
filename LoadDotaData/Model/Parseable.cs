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

        public Ability ActiveAbility { get; set; } = new Ability();
        public List<Effect> Effects { get; set; } = new List<Effect>();

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();
            ApplyEntries(retVal, data); //set header-level entries
            data.Sections.ForEach(s => ParseSection(retVal, s));
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
            //start by creating a hierarchal structure of all this data, discarding the top level structure
            var data = new Section(typeof(T).Name, text)?.Sections;
            return data == null ? new List<T>() : data.Select(d => ParseItem<T>(d)).ToList();
        }

        [SpecialHandlerSectionMethod("AbilitySpecial")]
        public void ParseAbilitySpecial(Section s)
        {
            var entries = s.GetAllEntries();

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
                string line = s.Replace("\"", ""); //no quotes
                int commentStart = s.IndexOf(COMMENT_IND);
                if (commentStart == 0) continue;
                if (commentStart > 0) line = s.Substring(0, commentStart).Trim();
                if (line.Length == 0) continue;

                //get non-blank line values
                string[] values = line.Split('\t')
                                      .Select(l => l.Trim())
                                      .Where(l => string.IsNullOrEmpty(l))
                                      .ToArray();

                //create an entry for dual values
                if (values.Count() == 2)
                {
                    Entries.Add(new Entry()
                    {
                        Title = values[0],
                        Value = values[1]
                    });
                    continue;
                }

                //deal with brackets
                if (line == "{")
                {
                    if (currentBracketLevel > 0) buffer.Add(line); //keep brackets that are inside sections
                    currentBracketLevel++;
                }

                if (line == "}")
                {
                    if (currentBracketLevel == 1)
                    {
                        Sections.Add(new Section(bufferName, buffer.ToArray()));
                        buffer.Clear();
                    }
                    currentBracketLevel--;
                }

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
        private string title = string.Empty;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                ReadMeta();
            }
        }
        public string Value { get; set; }

        public string ValueDest { get; set; } = string.Empty;
        public decimal NumericValue { get; set; } = 0;
        public bool IsNumericValue { get; set; } = false;
        public bool IsPercentage { get; set; } = false;
        public EffectClass AssociatedEffect { get; set; } = EffectClass.None;
        public bool ActiveEffect { get; set; } = false;
        public (string name, string dest)[] ExpectedEntries { get; set; }
        public bool IsDuration { get; set; } = false;

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
                AssociatedEffect = (EffectClass)Enum.Parse(typeof(EffectClass), matchingEffect.Name, false);

                //get the property where the value should be assigned
                ValueDest = matchingEffect.GetCustomAttribute<ValueDest>()?.DestProperty ?? string.Empty;                

                //Parse the value if needed
                if (decimal.TryParse(Value, out decimal d))
                {
                    IsNumericValue = true;
                    NumericValue = d;
                }
                
                IsPercentage = matchingEffect.GetCustomAttribute<PercentEffect>() != null;
                ActiveEffect = matchingEffect.GetCustomAttribute<ActiveEffect>() != null;
                ExpectedEntries = matchingEffect.GetCustomAttributes<ExpectedEntry>().Select(a => (a.Indicator, a.DestField)).ToArray();
            }

        }
    }
}
