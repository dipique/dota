using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotA.Model
{
    public static class Utils
    {
        /// <summary>
        /// Turns "BaseAgiGain" into "Base Agi Gain"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SeparateByCapital(string input)
        {
            //if it's all capital (like "ID"), just return it.
            if (!input.Any(c => Char.IsLetter(c) && Char.ToUpper(c) != c)) return input;

            List<char> retVal = new List<char>();
            foreach (var c in input)
            {
                if (Char.IsLetter(c) && Char.ToUpper(c) == c)
                {
                    retVal.Add(' ');
                }
                retVal.Add(c);
            }
            return new string(retVal.ToArray()).Trim();
        }
    }
}
