using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using DotA.Model;
using DotA.Flashcards;

namespace DotA
{
    public class DotaData
    {
        const string DEF_SAVE_LOC = "data.dat";

        public List<Hero> Heroes { get; set; } = new List<Hero>();
        public List<Item> Items { get; set; } = new List<Item>();

        public List<QuestionResponse> QuestionResponses { get; set; } = new List<QuestionResponse>();
        public List<FlashcardMode> Modes { get; set; } = new List<FlashcardMode>();
        public List<CustomQuestion> CustomQuestions { get; set; } = new List<CustomQuestion>();

        public FlashcardMode CurrentMode { get; set; } = new FlashcardMode();

        public static void Save(DotaData data)
        {
            XmlSerializer s = new XmlSerializer(typeof(DotaData));
            FileStream fs = new FileStream(DEF_SAVE_LOC, FileMode.OpenOrCreate);
            s.Serialize(fs, data);
            fs.Close();
        }

        public static DotaData Load()
        {
            if (!File.Exists(DEF_SAVE_LOC)) return new DotaData();

            XmlSerializer s = new XmlSerializer(typeof(DotaData));
            FileStream fs = new FileStream(DEF_SAVE_LOC, FileMode.Open);
            var retVal = (DotaData)s.Deserialize(fs);
            fs.Close();
            return retVal;
        }
    }
}
