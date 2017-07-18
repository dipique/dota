using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DotA.WebEdit.Models
{
    public class DynMultiView<T> : DynView<T> where T : class
    {
        public List<T> Objects { get; set; }

        public DynMultiView(List<T> objects)
        {
            srcType = typeof(T);
            Objects = objects;
        }
    }

    public class DynSingleView<T> : DynView<T> where T: class
    {        
        public T Object { get; set; }
        public DynSingleView(T obj)
        {
            srcType = typeof(T);
            Object = obj;
        }
    }

    /// <summary>
    /// The inheritance is mostly to make sure the displayvalue generation code
    /// won't be repeated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DynView<T> where T : class
    {
        protected Type srcType = null;

        private List<DisplayValue> displayValues = null;
        public List<DisplayValue> DisplayValues
        {
            get
            {
                if (displayValues == null)
                    displayValues = srcType.GetProperties().Where(p => p.CanRead)
                                                           .Select(p => new DisplayValue(p)).ToList();
                return displayValues;
            }
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