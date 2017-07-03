using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using DotA.Model.Enums;
using DotA.Parsing;

namespace DotA.Model
{
    public abstract class Parseable
    {
        
        public string Name { get; set; }

        [JID("ID")]
        public string ID { get; set; }

        public string ImgName { get; set; }

        /// <summary>
        /// Stats provided
        /// </summary>
        public decimal Agility => Effects.Where(e => e.Class == EffectClass.Agility).Sum(e => e.Amount);
        public decimal Strength => Effects.Where(e => e.Class == EffectClass.Strength).Sum(e => e.Amount);
        public decimal Intelligence => Effects.Where(e => e.Class == EffectClass.Intelligence).Sum(e => e.Amount);

        public List<Effect> Effects { get; set; } = new List<Effect>();

        public static T ParseItem<T>(Section data) where T : Parseable
        {
            T retVal = Activator.CreateInstance<T>();

            //get tagged properties
            var taggedProperties = typeof(T).GetProperties()
                                            .Where(p => p.GetCustomAttributes(typeof(JID))
                                                         .Any());

            //start by assigning any attributes available
            foreach (Entry e in data.Entries)
            {
                //get a matching property
                var matchingProp = taggedProperties.FirstOrDefault(p => ((JID)p.GetCustomAttribute(typeof(JID))).ID == e.Title);
                if (matchingProp != null)
                {
                    if (matchingProp.PropertyType == typeof(decimal))
                    {
                        decimal val = decimal.TryParse(e.Value, out decimal d) ? d : 0;
                        matchingProp.SetValue(retVal, val);
                    }
                    else
                    {
                        matchingProp.SetValue(retVal, e.Value);
                    }                    
                }
            }


        }
    }
}
