using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using DotA.Model;
using DotA.Flashcards;

namespace DotA
{
    public class DotaData
    {
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

        public static void Save(DotaData data, string filename = "dota.dat")
        {
            XmlSerializer s = new XmlSerializer(typeof(DotaData));
            if (File.Exists(filename)) File.Delete(filename);
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            s.Serialize(fs, data);
            fs.Close();
        }

        public static DotaData Load(string filename = "dota.dat")
        {
            if (!File.Exists(filename)) return new DotaData();

            XmlSerializer s = new XmlSerializer(typeof(DotaData));
            FileStream fs = new FileStream(filename, FileMode.Open);
            var retVal = (DotaData)s.Deserialize(fs);
            fs.Close();
            return retVal;
        }
    }
}
