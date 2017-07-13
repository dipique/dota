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
        public const int MAX_ID = 255; //dota IDs only go up to 255
        private const char BEHAVIOR_SEP = '|';

        public virtual string Name { get; set; }       
        
        [JID("ID")]
        public virtual string ID { get; set; }

        public string ImgName { get; set; }

        public bool Valid { get; set; } = true;

        public virtual object[] ParseableCandidates { get; }

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();
            retVal.Name = data.Name;
            data.Sections.ForEach(s => ParseSection(retVal, s));

            //apply all the top level entries. We do this after the section application because sometimes these entries
            //need to be applied to effects created by those sections.
            foreach (var entry in data.Entries.Where(e => !e.IsNumericValue || e.NumericValue != 0))
                entry.Apply(retVal, retVal.ParseableCandidates); //apply to the active effect if the item doesn't have a matching parameter
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
    }
}
