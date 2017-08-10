using System.IO;
using System.Linq;

using DotA.Model;

namespace DotA.ParseTool
{
    class Program
    {
        private const string itemLocation = "data\\items.txt";
        private const string heroLocation = "data\\npc_heroes.txt";
        private const string abilityLocation = "data\\npc_abilities.txt";
        private const string saveLocation = "dota.dat";
        static void Main(string[] args)
        {
            DotaData dd = new DotaData();
            dd.Items = Parseable.ParseItems<Item>(File.ReadAllLines(itemLocation)).Where(i => i.Valid).ToList();
            dd.Heroes = Parseable.ParseItems<Hero>(File.ReadAllLines(heroLocation)).Where(h => h.Valid).ToList();

            //get the abilities and try to assign them to the heroes
            var abilities = Parseable.ParseItems<Ability>(File.ReadAllLines(abilityLocation));
            dd.Heroes.ForEach(h => {
                foreach (var abilityName in h.AbilityList) {
                    var abilityMatch = abilities.FirstOrDefault(a => a.Name == abilityName);
                    if (abilityMatch != null)
                        h.Abilities.Add(abilityMatch);
                }

                //Try to assign the talents as well
                if (h.TalentList.Count() != 8) return; // 8 is what it should be

                h.Talents.Add(new Talent() {
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


            DotaData.Save(dd, saveLocation);
        }
    }
}
