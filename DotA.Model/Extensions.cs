using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotA.Model.Extensions
{
    public static class Extensions
    {
        public static T GetValue<T>(this PropertyInfo p, object src) => (T)p.GetValue(src);
    }
}
