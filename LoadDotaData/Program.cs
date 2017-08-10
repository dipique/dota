using System.IO;
using System.Linq;

using DotA.Model;

namespace DotA.ParseTool
{
    class Program
    {
        private const string itemLocation = @"c:\Data\dota\items.txt";
        private const string heroLocation = @"c:\Data\dota\npc_heroes.txt";
        private const string abilityLocation = @"c:\Data\dota\npc_abilities.txt";
        private const string saveLocation = @"c:\Data\dota\dota.dat";
        static void Main(string[] args)
        {
            var dd = new DotAData(itemLocation, heroLocation, abilityLocation);
            dd.Save(saveLocation);
        }
    }
}
