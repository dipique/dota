using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DotA;
using DotA.Model;

namespace LoadDotaData
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
                if (h.TalentList.Count() == 8) //that's what it should be
                {
                    h.Talents = new Talent[] {
                        new Talent() {
                            Level = 10,
                            Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[0])?.MainEffect,
                            Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[1])?.MainEffect
                        },
                        new Talent() {
                            Level = 15,
                            Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[2])?.MainEffect,
                            Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[3])?.MainEffect
                        },
                        new Talent() {
                            Level = 20,
                            Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[4])?.MainEffect,
                            Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[5])?.MainEffect
                        },
                        new Talent() {
                            Level = 25,
                            Option1 = abilities.FirstOrDefault(a => a.Name == h.TalentList[6])?.MainEffect,
                            Option2 = abilities.FirstOrDefault(a => a.Name == h.TalentList[7])?.MainEffect
                        }
                    };
                }
            });


            DotaData.Save(dd, saveLocation);
        }
    }
}
