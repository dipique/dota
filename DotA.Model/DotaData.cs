using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using DotA.Model;
using DotA.Flashcards;
using DotA.Model.Enums;

namespace DotA
{
    public class DotAData
    {
        //Right now I'm using this so it's globally accessible. TODO: Find a better way.
        public static Config Config { get; set; } = new Config();
        private const string CFG_SUFFIX = ".config";
        public static List<EffectClass> ECs => Config.EffectClasses; //just a shortcut for pithy coding

        public List<Hero> Heroes { get; set; } = new List<Hero>();
        public List<Item> Items { get; set; } = new List<Item>();

        public IEnumerable<Ability> Abilities => Heroes.SelectMany(h => h.Abilities);
        public IEnumerable<Parseable> GetParseableObjects => Heroes.Cast<Parseable>().Concat(Items);
        public Parseable GetParseablebyName(string name) => GetParseableObjects.Select(o => o.GetItemByName(name))
                                                                               .FirstOrDefault(o => o != null);


        public List<QuestionResponse> QuestionResponses { get; set; } = new List<QuestionResponse>();
        public List<FlashcardMode> Modes { get; set; } = new List<FlashcardMode>();
        public List<CustomQuestion> CustomQuestions { get; set; } = new List<CustomQuestion>();

        public FlashcardMode CurrentMode { get; set; } = new FlashcardMode();

        public void Save(string filename)
        {
            XmlSerializer s = new XmlSerializer(typeof(DotAData));
            if (File.Exists(filename)) File.Delete(filename);
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            s.Serialize(fs, this);
            fs.Close();

            ////save config file
            //s = new XmlSerializer(typeof(Config));
            //string cfgFilename = $"{filename}{CFG_SUFFIX}";
            //if (File.Exists(cfgFilename)) File.Delete(cfgFilename);
            //fs = new FileStream(cfgFilename, FileMode.OpenOrCreate);
            //s.Serialize(fs, Config);
            //fs.Close();
        }

        public static DotAData LoadFromFile(string filename)
        {
            //config load
            string cfgFilename = $"{filename}{CFG_SUFFIX}";
            if (!File.Exists(cfgFilename)) return new DotAData();
            var s = new XmlSerializer(typeof(Config));
            var fs = new FileStream(cfgFilename, FileMode.Open);
            Config = (Config)s.Deserialize(fs);
            fs.Close();

            if (!File.Exists(filename)) return new DotAData();

            s = new XmlSerializer(typeof(DotAData));
            fs = new FileStream(filename, FileMode.Open);
            var retVal = (DotAData)s.Deserialize(fs);
            fs.Close();

            return retVal;
        }

        public DotAData() { }
        public DotAData(string itemLocation, string heroLocation, string abilityLocation, string saveLocation = null)
        {
            Items = Parseable.ParseItems<Item>(File.ReadAllLines(itemLocation)).Where(i => i.Valid).ToList();
            Heroes = Parseable.ParseItems<Hero>(File.ReadAllLines(heroLocation)).Where(h => h.Valid).ToList();

            //get the abilities and try to assign them to the heroes
            var abilities = Parseable.ParseItems<Ability>(File.ReadAllLines(abilityLocation));
            Heroes.ForEach(h => {
                foreach (var abilityName in h.AbilityList)
                {
                    var abilityMatch = abilities.FirstOrDefault(a => a.Name == abilityName);
                    if (abilityMatch != null)
                        h.Abilities.Add(abilityMatch);
                }

                //Try to assign the talents as well
                if (h.TalentList.Count() != 8) return; // 8 is what it should be

                h.Talents.Add(new Talent()
                {
                    Level = 10,
                    Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[0])?.Effects.FirstOrDefault(),
                    Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[1])?.Effects.FirstOrDefault()
                });
                h.Talents.Add(new Talent()
                {
                    Level = 15,
                    Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[2])?.Effects.FirstOrDefault(),
                    Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[3])?.Effects.FirstOrDefault()
                });
                h.Talents.Add(new Talent()
                {
                    Level = 20,
                    Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[4])?.Effects.FirstOrDefault(),
                    Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[5])?.Effects.FirstOrDefault()
                });
                h.Talents.Add(new Talent()
                {
                    Level = 25,
                    Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[6])?.Effects.FirstOrDefault(),
                    Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[7])?.Effects.FirstOrDefault()
                });

            });

            if (!string.IsNullOrEmpty(saveLocation))
                Save(saveLocation);
        }

        public static string[] GetOptions(SelectionOptions optionSet)
        {
            switch (optionSet)
            {
                case SelectionOptions.EffectClass:
                    return ECs.Select(ec => ec.Name).ToArray();
                default: return null;
            }
        }
    }

    public class Config
    {
        public List<EffectClass> EffectClasses = new List<EffectClass>();
    }
}
