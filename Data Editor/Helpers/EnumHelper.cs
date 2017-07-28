using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotA.WebEdit.Helpers
{
    public static class EnumHelper
    {
        //https://stackoverflow.com/questions/4194333/mvc-2-passing-enum-to-checkboxfor?noredirect=1&lq=1
        public static object ParseToEnumFlag(Type enumType, NameValueCollection source, string formKey)
        {
            //MVC 'helpfully' parses the checkbox into a comma-delimited list. We pull that out and sum the values after parsing it back into the enum.
            return Enum.ToObject(enumType, source.Get(formKey).ToIEnumerable<int>(',').Sum());
        }

        public static IEnumerable<T> ToIEnumerable<T>(this string source, char delimiter)
        {
            return source == null ? new T[] { default(T) }
                                  : source.Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(x => (T)Convert.ChangeType(x, typeof(T)));
        }
    }
}