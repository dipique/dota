using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using DotA.Model;
using DotA.Model.Enums;

namespace DotA.Parsing

//Parsing from VPK files - instructions
//https://puppet-master.net/tutorials/source-engine/extract-content-from-vpk-files/
{
    class DotAParser
    {
        private const string START_IND = "\t//=================================================================================================================";



        //https://raw.githubusercontent.com/kil0gram/dota2/initmaster/dotadata/Model/ItemsClass.cs
        //This was used as a starting point for this method, but had to be re-written for the most part
        public static List<T> ParseItems<T>(string[] text) where T : Parseable
        {
            //list to hold our parsed items.
            var items = new List<T>();

            //start by creating a hierarchal structure of all this data
            Section data = new Section(typeof(T).Name, text);


            //tracks the status within the file
            var currentLineType = LineType.None;

            //item object will will be populating
            T item = (T)Activator.CreateInstance(typeof(T));

            //lets go line by line to start parsing.
            foreach (string line in text)
            {
                //clean up the text, remove quotes.
                string line_noquotes = line.Replace("\"", "");
                string line_clean = line_noquotes.Replace("\t", "").Replace("_", " ").Trim();

                //Get the line type
                if (line_clean.StartsWith(START_IND))
                {
                    currentLineType = currentLineType == LineType.NewItemIndBot ? LineType.ItemData 
                                                                                : LineType.ItemTitle;
                    continue;
                }

                //Handle the title line
                if (currentLineType == LineType.ItemTitle)
                {
                    item = Activator.CreateInstance<T>();
                    item.Name = line_clean.Replace("/", "").Trim();
                    currentLineType = LineType.NewItemIndBot;
                    continue;
                }

                //if we aren't in an item we need to wait for a title
                if (currentLineType == LineType.None) continue;

                //try to parse as a top-level item


            }

            return items;
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
                string line = s;
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
                    Entries.Add(new Entry() {
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
