using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using DotA.Model.Attributes;

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
        /// Call strings can either be in the form Property or Property[i] if an
        /// index, or Property[uniqueStringID] if a collection such that the attribute PrimaryKey
        /// is sufficient to find the correct item in the list.
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
                target = prop.IndexType == IndexType.String ? GetPKMatchValue(target, prop, propertyToGet)
                                                            : propertyToGet.GetValue(target, prop.IndexAsObjectArray);
            }

            var setProp = new PropertyCall(hierarchy.Last());
            PropertyInfo propertyToSet = target.GetType().GetProperty(setProp.PropertyName);
            propertyToSet.SetValue(target, value, setProp.IndexAsObjectArray);
        }

        private static object GetPKMatchValue(object target, PropertyCall prop, PropertyInfo propertyToGet)
        {
            //first, get the enumerable target
            var enumerableObj = propertyToGet.GetValue(target, null); //this is now an array, like Ability.Effects

            //get the generic type of the target
            Type genType = enumerableObj.GetType().GetGenericArguments()[0]; //this is now the type, like Effect

            //get the property designated as the PrimaryKey using that attribute
            var pkProp = genType.GetProperties().FirstOrDefault(pi => pi.GetCustomAttribute<PrimaryKey>() != null);

            //create the expression
            Func<object, bool> comparePK = new Func<object, bool>(o => pkProp.GetValue(o).ToString() == prop.StrIndex);

            //Get the method that represents FirstOrDefault for the enumerable target
            var fodMethod = enumerableObj.GetType().GetMethods()
                                                   .Where(m => m.Name == nameof(Enumerable.FirstOrDefault))
                                                   .FirstOrDefault(m => m.GetParameters().Count() == 1);

            //use the expression as an input to the method call
            return fodMethod.Invoke(enumerableObj, new object[] { comparePK });
        }

        public class PropertyCall
        {
            private const int NOT_FOUND = -1;
            public int Index { get; set; } = NOT_FOUND;
            public string StrIndex { get; set; }
            public string PropertyName { get; set; }
            public bool HasIndex => IndexType == IndexType.None;
            public IndexType IndexType { get; set; } = IndexType.None;

            public object[] IndexAsObjectArray => HasIndex ? null : new object[] { Index };

            /// <summary>
            /// Call strings can either be in the form Property or Property[i] if an
            /// index, or Property[uniqueStringID] if a collection such that the attribute PrimaryKey
            /// is sufficient to find the correct item in the list.
            /// </summary>
            /// <param name="callString"></param>
            public PropertyCall(string callString)
            {
                //Get the index, if there is one
                var openBracketPosition = callString.IndexOf('[');
                var closedBracketPosition = callString.IndexOf(']');
                var indexLength = closedBracketPosition - openBracketPosition - 1;
                var indexString = callString.Substring(openBracketPosition + 1, indexLength);

                //These conditions indicate that there isn't an index
                if (openBracketPosition == NOT_FOUND || closedBracketPosition == NOT_FOUND || openBracketPosition > closedBracketPosition)
                {
                    PropertyName = callString;
                    return;
                }

                PropertyName = callString.Substring(0, openBracketPosition);

                //If it's an integer index
                if (int.TryParse(indexString, out int index))
                {
                    Index = index;
                    IndexType = IndexType.Int;
                }
                else //it's a string index
                {
                    StrIndex = indexString;
                    IndexType = IndexType.String;
                }
            }
        }

        public enum IndexType
        {
            None = 0,
            Int,
            String
        }
    }
}
