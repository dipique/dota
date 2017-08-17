using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DotA.Model.Extensions
{
    public static class Extensions
    {
        public static T GetValue<T>(this PropertyInfo p, object src) => (T)p.GetValue(src);

        /// <summary>
        /// This method is different because it will cast the generic parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static List<T> GetListValue<T>(this PropertyInfo p, object src)
        {
            if (!p.PropertyType.IsList()) return null;

            List<T> retVal = new List<T>();
            var stupidList = p.GetValue(src);
            foreach (var o in (stupidList as IList))
            {
                retVal.Add((T)o);
            }
            return retVal;
        }

        public static bool IsList(this Type type) => type.IsGenericType &&
                                                     typeof(IEnumerable).IsAssignableFrom(type);
    }
}
