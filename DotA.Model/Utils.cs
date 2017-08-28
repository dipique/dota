using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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


        /// <summary>
        /// TODO: This needs to support array/list indexes as well as nagivating to the correct effect using the "unique" ID
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public static void SetCompoundProperty(List<string> hierarchy, object target, object value)
        {
            for (int i = 0; i < hierarchy.Count() - 1; i++)
            {
                var prop = new PropertyCall(hierarchy[i]);
                PropertyInfo propertyToGet = target.GetType().GetProperty(prop.PropertyName);
                target = propertyToGet.GetValue(target, prop.IndexAsObjectArray);
            }

            var setProp = new PropertyCall(hierarchy.Last());
            PropertyInfo propertyToSet = target.GetType().GetProperty(setProp.PropertyName);
            propertyToSet.SetValue(target, value, setProp.IndexAsObjectArray);
        }

        public class PropertyCall
        {
            private const int NOT_FOUND = -1;
            public int Index { get; set; } = NOT_FOUND;
            public string PropertyName { get; set; }
            public bool HasIndex => Index == NOT_FOUND;

            public object[] IndexAsObjectArray => HasIndex ? null : new object[] { Index };

            /// <summary>
            /// Call string scan either be in the form Property or Property[i] if an
            /// index.
            /// </summary>
            /// <param name="callString"></param>
            public PropertyCall(string callString)
            {
                var openBracketPosition = callString.IndexOf('[');
                var closedBracketPosition = callString.IndexOf(']');
                var indexLength = closedBracketPosition - openBracketPosition - 1;

                if (openBracketPosition != NOT_FOUND && closedBracketPosition != NOT_FOUND && 
                    openBracketPosition < closedBracketPosition &&
                    int.TryParse(callString.Substring(openBracketPosition + 1, indexLength), out int index))
                {
                    Index = index;
                    PropertyName = callString.Substring(0, openBracketPosition);                    
                }
                else
                {
                    PropertyName = callString;
                }
            }
        }
    }
}
