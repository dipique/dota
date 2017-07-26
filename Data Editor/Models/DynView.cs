﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Mvc;

using DotA.Model.Attributes;
using DotA.Model;

namespace DotA.WebEdit.Models
{
    public class DynMultiView<T> : DynView<T> where T : class
    {
        public List<T> Items { get; set; }

        public DynMultiView(List<T> objects)
        {
            srcType = typeof(T);
            Items = objects;
        }
    }

    public class DynSingleView<T> : DynView<T> where T : class
    {
        public T Item { get; set; }
        public DynSingleView(T obj)
        {
            srcType = typeof(T);
            Item = obj;
        }

        public DynSingleView()
        {
            srcType = typeof(T);
            Item = Activator.CreateInstance<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="index">Index of the array, if applicable</param>
        /// <returns></returns>
        public Expression<Func<DynSingleView<T>, TType>> GetExpression<TType>(DisplayValue dv, int index = 0)
        {
            var param = Expression.Parameter(typeof(DynSingleView<T>));
            var instance = Expression.Property(param, nameof(DynSingleView<T>.Item));
            var propertyCall = Expression.Property(instance, dv.PropertyName);

            if (typeof(TType) == typeof(Enum))
            {
                var convert = Expression.Convert(propertyCall, typeof(TType));
                var lambda2 = Expression.Lambda<Func<DynSingleView<T>, TType>>(convert, param);
                var o = new {
                    Name = (TType)lambda2.Compile()(this)
                };
                var anonParam = Expression.Constant(o, o.GetType());
                var anonProperty = Expression.Property(anonParam, nameof(o.Name));
                var lambda3 = Expression.Lambda<Func<DynSingleView<T>, TType>>(anonProperty, param);
                return lambda3;
            }

            var lambda = !dv.SrcProperty.PropertyType.IsArray ? Expression.Lambda<Func<DynSingleView<T>, TType>>(propertyCall, param)
                                                              : Expression.Lambda<Func<DynSingleView<T>, TType>>(Expression.ArrayIndex(propertyCall, Expression.Constant(index)), param);
            return lambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="index">Index of the array, if applicable</param>
        /// <returns></returns>
        public Expression<Func<DynSingleView<T>, Enum>> GetExpressionEnum(DisplayValue dv)
        {
            var param = Expression.Parameter(typeof(DynSingleView<T>));
            var instance = Expression.Property(param, nameof(DynSingleView<T>.Item));
            var propertyCall = Expression.Property(instance, dv.PropertyName);

            var lambda = Expression.Lambda<Func<DynSingleView<T>, Enum>>(propertyCall, param);
            return lambda;
        }

    }

    /// <summary>
    /// The inheritance is mostly to make sure the displayvalue generation code
    /// won't be repeated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DynView<T> where T : class
    {
        private const string IMAGE_FOLDER = @"C:\o\OneDrive - Inspired\dev\DotA Flashcards\LoadDotaData\data\img\"; //todo: add to web.config

        protected Type srcType = null;
        public string Title { get; set; }
        private List<DisplayValue> displayValues = null;
        public List<DisplayValue> DisplayValues
        {
            get
            {
                if (displayValues == null)
                    displayValues = srcType.GetProperties().Where(p => p.CanRead)
                                                           .Where(p => !p.PropertyType.IsGenericType)
                                                           .Where(p => p.GetCustomAttribute<NoDisplay>() == null)
                                                           .Select(p => new DisplayValue(p))
                                                           .OrderBy(dv => dv.DisplayOrder)
                                                           .ThenBy(dv => dv.DisplayGroup).ToList();
                return displayValues;
            }
        }

        private string imageFolder = string.Empty;
        public string ImageFolder
        {
            get
            {
                if (string.IsNullOrEmpty(imageFolder))
                {
                    string working = IMAGE_FOLDER;
                    string subFolder = typeof(T).GetCustomAttribute<ImageFolder>()?.FolderName;
                    if (!string.IsNullOrEmpty(subFolder))
                        working += subFolder + "\\";
                    imageFolder = working;
                }
                return imageFolder;
            }
            set
            {
                imageFolder = value;
            }
        }

        public string GetImage(string filename)
        {
            if (!File.Exists(ImageFolder + filename)) return string.Empty;

            var base64 = Convert.ToBase64String(File.ReadAllBytes(ImageFolder + filename));
            var imgSrc = String.Format($"data:image/png;base64,{base64}");
            return imgSrc;
        }
    }

    public class DisplayValue
    {
        public PropertyInfo SrcProperty { get; set; } = null;
        public string PropertyName => SrcProperty?.Name;
        public bool Editable { get; set; } = true;
        public int FieldOrder { get; set; }

        public DisplayValue(PropertyInfo property)
        {
            SrcProperty = property;
            Editable = property.GetCustomAttribute<DisplayOnly>() == null;
        }

        public DisplayValueType Type
        {
            get
            {
                if (SrcProperty.PropertyType == typeof(string)) return DisplayValueType.String;
                if (SrcProperty.PropertyType == typeof(decimal)) return DisplayValueType.Decimal;
                if (SrcProperty.PropertyType.IsArray) return DisplayValueType.DecimalArray;
                if (SrcProperty.PropertyType.IsEnum) return SrcProperty.PropertyType.GetCustomAttribute<FlagsAttribute>() == null ? DisplayValueType.PickList
                                                                                                                                  : DisplayValueType.PickList_Multi;
                return DisplayValueType.Other;
            }
        }

        public string[] GetPicklistOptions() => Type != DisplayValueType.PickList 
                                             && Type != DisplayValueType.PickList_Multi ? null
                                                                                        : Enum.GetNames(SrcProperty.PropertyType);
        public List<SelectListItem> PicklistOptionsAsListItems(object src)
        {
            var selections = GetValue(src).Split(',').Select(s => s.Trim());
            return GetPicklistOptions().Select(s => new SelectListItem() {
                Selected = selections.Any(sel => sel == s),
                Text = s
            }).OrderByDescending(sl => sl.Selected).ToList();
        }

        public string GetValue(object src)
        {
            object value = null;
            //try {
            value = SrcProperty.GetValue(src);
            //} catch { }

            switch (Type)
            {                
                case DisplayValueType.DecimalArray:
                    return string.Join(" ", ((decimal[])value).Select(d => string.Format("{0:N}", d)));
                case DisplayValueType.Decimal:
                    return string.Format("{0:N}", value);
                case DisplayValueType.PickList_Multi:
                default:
                    return value?.ToString();    //TODO: Handle flags
            }
        }

        public bool SetValue(object item, string val)
        {
            object value = null;

            try
            {
                switch (Type)
                {
                    case DisplayValueType.DecimalArray:
                        value = val.Split(' ').Select(s => decimal.Parse(s)).ToArray();
                        break;
                    case DisplayValueType.Decimal:
                        value = decimal.Parse(val);
                        break;
                    case DisplayValueType.PickList:
                    case DisplayValueType.PickList_Multi:
                        value = Enum.Parse(SrcProperty.PropertyType, val);
                        break;
                    default:
                        value = val;
                        break;
                }

                //If we have a value, set it
                SrcProperty.SetValue(item, value);
                return true;
            }
            catch { return false; } //conversion unsuccessful
        }

        private string propertyDisplayName = string.Empty;
        public string PropertyDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(propertyDisplayName))
                {
                    //first try to get it from the attribute, then by capital separation
                    var attrText = SrcProperty.GetCustomAttribute<DisplayText>()?.Text;
                    propertyDisplayName = attrText ?? Utils.SeparateByCapital(SrcProperty.Name);
                }
                return propertyDisplayName;
            }
        }


        public int DisplayOrder
        {
            get
            {
                if (displayOrder == int.MaxValue)
                {
                    displayOrder = SrcProperty.GetCustomAttribute<FieldOrder>(true)?.Order ?? int.MaxValue - 1; //the subtraction here is purely to mark it as attempted
                }
                return displayOrder;
            }
        }

        public string DisplayGroup
        {
            get
            {
                if (displayGroup == null)
                {
                    displayGroup = SrcProperty.GetCustomAttribute<FieldOrder>(true)?.GroupName ?? string.Empty; //null is unchecked, string.empty is checked
                }
                return displayGroup;
            }
        }
        private string displayGroup = null;
        private int displayOrder = int.MaxValue;
    }

    public enum DisplayValueType
    {
        Other = 0,
        Decimal,
        DecimalArray,
        String,
        PickList, //enum
        PickList_Multi //flags
    }
}