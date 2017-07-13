using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotA;
using DotA.Model;

namespace LoadDotaData
{
    class Program
    {
        private const string itemLocation = "data\\items.txt";
        private const string saveLocation = "dota.dat";
        static void Main(string[] args)
        {
            DotaData dd = new DotaData();
            dd.Items = Parseable.ParseItems<Item>(File.ReadAllLines(itemLocation));


            DotaData.Save(dd, saveLocation);

        }
    }
}
