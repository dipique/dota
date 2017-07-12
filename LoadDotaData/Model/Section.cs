using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DotA.Model.Enums;
using DotA.Model.Attributes;

namespace DotA.Model
{
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
