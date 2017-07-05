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

        public string Name { get; set; }

        [JID("ID")]
        public string ID { get; set; }

        public string ImgName { get; set; }

        public bool Valid { get; set; } = true;

        /// <summary>
        /// Stats provided
        /// </summary>
        public decimal Agility => Effects.Where(e => e.Class == EffectClass.Agility).Sum(e => e.Amount);
        public decimal Strength => Effects.Where(e => e.Class == EffectClass.Strength).Sum(e => e.Amount);
        public decimal Intelligence => Effects.Where(e => e.Class == EffectClass.Intelligence).Sum(e => e.Amount);

        public List<Effect> Effects { get; set; } = new List<Effect>();

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();

            //get tagged properties
            var taggedProperties = typeof(T).GetProperties()
                                            .Where(p => p.GetCustomAttributes(typeof(JID))
                                                         .Any());

            //start by assigning any attributes available
            foreach (Entry e in data.Entries)
            {
                //get a matching property
                var matchingProp = taggedProperties.FirstOrDefault(p => ((JID)p.GetCustomAttribute(typeof(JID))).IDs.Any(id => id == e.Title));
                if (matchingProp != null)
                {
                    if (matchingProp.PropertyType == typeof(decimal))
                    {
                        decimal val = decimal.TryParse(e.Value, out decimal d) ? d : 0;
                        matchingProp.SetValue(retVal, val);
                    }
                    else
                    {
                        matchingProp.SetValue(retVal, e.Value);
                    }                    
                }
            }


        }

        public static List<T> ParseItems<T>(string[] text) where T : Parseable
        {
            //start by creating a hierarchal structure of all this data, discarding the top level structure
            var data = new Section(typeof(T).Name, text)?.Sections;
            return data == null ? new List<T>() : data.Select(d => Parseable.ParseItem<T>(d)).ToList();
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
    }

    public class Entry
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
