using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DotA.WebEdit.Models
{
    public class DynMultiView<T> where T : class
    {
        private Type srcType = null;
        public List<DisplayValue> DisplayValues { get; private set; }
        public List<T> Objects { get; set; }

        public DynMultiView(List<T> objects)
        {
            srcType = typeof(T);
            Objects = objects;
            DisplayValues = srcType.GetProperties().Where(p => p.CanRead)
                                                   .Where(p => !p.PropertyType.IsGenericType)
                                                   .Select(p => new DisplayValue(p)).ToList();
        }
    }

    public class DynSingleView<T> where T : class
    {
        private Type srcType = null;
        public List<DisplayValue> DisplayValues { get; private set; }
        public T Object { get; set; }

        public DynSingleView(T obj)
        {
            srcType = typeof(T);
            Object = obj;
            DisplayValues = srcType.GetProperties().Where(p => p.CanRead)
                                                   .Select(p => new DisplayValue(p)).ToList();
        }
    }

    public class DisplayValue
    {
        private PropertyInfo srcProperty = null;
        public string PropertyName => srcProperty?.Name;

        public DisplayValue(PropertyInfo property)
        {
            srcProperty = property;
        }

        public DisplayValueType Type
        {
            get
            {
                if (srcProperty.PropertyType == typeof(string)) return DisplayValueType.String;
                if (srcProperty.PropertyType == typeof(decimal)) return DisplayValueType.Decimal;
                if (srcProperty.PropertyType.IsArray) return DisplayValueType.DecimalArray;
                if (srcProperty.PropertyType.IsEnum) return DisplayValueType.PickList;
                return DisplayValueType.Other;
            }
        }

        public string[] GetPicklistOptions() => Type != DisplayValueType.PickList ? null
                                                                                  : Enum.GetNames(srcProperty.PropertyType);
        public string GetValue(object src)
        {
            object value = null;
            //try {
            value = srcProperty.GetValue(src);
            //} catch { }

            switch (Type)
            {                
                case DisplayValueType.DecimalArray: return string.Join(" ", (decimal[])value);
                default: return value?.ToString();    //TODO: Handle flags
            }
        }
    }

    public enum DisplayValueType
    {
        Other = 0,
        Decimal,
        DecimalArray,
        String,
        PickList //enum
    }
}