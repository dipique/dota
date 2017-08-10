using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IO = System.IO;

using DotA;
using DotA.Model;
using DotA.WebEdit.Models;
using DotA.WebEdit.Helpers;
using DotA.Model.Extensions;

namespace DotA.WebEdit.Controllers
{
    public abstract class DotAController : Controller
    {
        /// <summary>
        /// Store the data in a persistent session
        /// </summary>
        private const string itemLocation = @"c:\Data\dota\items.txt";
        private const string heroLocation = @"c:\Data\dota\npc_heroes.txt";
        private const string abilityLocation = @"c:\Data\dota\npc_abilities.txt";
        private const string saveLocation = @"c:\Data\dota\dota.dat";
        internal const string dataInd = "data";
        public DotAData db
        {
            get
            {
                if ((DotAData)(Session[dataInd]) == null)
                    Session[dataInd] = IO.File.Exists(saveLocation) ? DotAData.LoadFromFile(saveLocation)
                                                                    : new DotAData(itemLocation, heroLocation, abilityLocation, saveLocation);
                return (DotAData)Session[dataInd];
            }
        }

        public ActionResult Image(string dir, string filename)
        {
            var path = IO.Path.Combine(dir, filename);
            return File(path, "image/png");
        }

        private void DeleteSourceData()
        {
            IO.File.Delete(saveLocation);
            ClearSourceCache();
        }

        private void ClearSourceCache()
        {
            Session[dataInd] = null;
        }

        public abstract ActionResult DefaultView(string itemName);
        public abstract ActionResult Index();

        public ActionResult ItemUpdate<T>(DynSingleView<T> model) where T : Parseable
        {
            //get the db list of with that object
            var allProps = typeof(DotAData).GetProperties();
            var dbListProp = allProps.FirstOrDefault(p => p.PropertyType?.GetGenericArguments()?.FirstOrDefault() == typeof(T));

            //get the item with the matching name
            var item = ((IEnumerable<T>)dbListProp.GetValue(db)).FirstOrDefault(i => i.Name == model.Item.Name);
            if (item == null) return View();

            //check all the values
            bool updated = false;
            foreach (var dv in model.DisplayValues.Where(dv => dv.Editable))
            {
                if (dv.Type == DisplayValueType.PickList_Multi)
                {
                    var newEnumValue = EnumHelper.ParseToEnumFlag(dv.SrcProperty.PropertyType, Request.Form, $"{dv.PropertyName}[]");
                    var oldEnumValue = dv.SrcProperty.GetValue(model.Item);
                    if (newEnumValue == oldEnumValue) continue;
                    updated = true;
                    dv.SrcProperty.SetValue(item, newEnumValue);
                }
                else
                {
                    //see if anything has changed
                    string newStrVal = dv.GetValueAsString(model.Item);
                    string oldStrVal = dv.GetValueAsString(item);
                    if (newStrVal == oldStrVal) continue;

                    //if it has changed, set the change
                    updated = true;
                    dv.SetValueFromString(item, newStrVal);
                }
            }

            if (updated)
            {
                db.Save(saveLocation);
                ClearSourceCache(); //forces data to be refreshed
            }

            //refresh the whole default page (the view will check the abilities for a match if there's no top level match)
            return DefaultView(model.Item.Name);
        }

        public virtual ActionResult EffectUpdate(DynSingleView<Effect> model)
        {
            //get the list of all effects
            var effects = db.Abilities.SelectMany(a => a.Effects)
                                      .Concat(db.Items.SelectMany(i => i.Ability.Effects));

            //get the item with the matching name & class
            var item = effects.FirstOrDefault(e => e.ParentName == model.Item.ParentName && e.Class == model.Item.Class);
            if (item == null) return View();

            //check all the values
            bool updated = false;
            foreach (var dv in model.DisplayValues.Where(dv => dv.Editable))
            {
                if (dv.Type == DisplayValueType.PickList_Multi)
                {
                    var newEnumValue = EnumHelper.ParseToEnumFlag(dv.SrcProperty.PropertyType, Request.Form, $"{dv.PropertyName}[]");
                    var oldEnumValue = dv.SrcProperty.GetValue(model.Item);
                    if (newEnumValue == oldEnumValue) continue;
                    updated = true;
                    dv.SrcProperty.SetValue(item, newEnumValue);
                }
                else
                {
                    //see if anything has changed
                    string newStrVal = dv.GetValueAsString(model.Item);
                    string oldStrVal = dv.GetValueAsString(item);
                    if (newStrVal == oldStrVal) continue;

                    //if it has changed, set the change
                    updated = true;
                    dv.SetValueFromString(item, newStrVal);
                }
            }

            if (updated)
            {
                db.Save(saveLocation);
                ClearSourceCache(); //forces data to be refreshed
            }

            //refresh the whole default page (the view will check the abilities for a match if there's no top level match)
            return DefaultView(model.Item.ParentName);
        }

        public virtual ActionResult DeleteEffect(DynSingleView<Effect> model)
        {
            //get the matching ability
            var abilities = db.Abilities.Concat(db.Items.Select(i => i.Ability));
            var matchingAbility = abilities.FirstOrDefault(a => a.Effects.Any(e => e.ParentName == model.Item.ParentName 
                                                                                && e.Class == model.Item.Class));
            if (matchingAbility == null) return View();
            var matchingEffect = matchingAbility.Effects.First(e => e.ParentName == model.Item.ParentName
                                                                                && e.Class == model.Item.Class);

            //delete the effect
            matchingAbility.Effects.Remove(matchingEffect);

            //save the changes
            db.Save(saveLocation);
            ClearSourceCache(); //forces data to be refreshed

            //refresh the whole default page (the view will check the abilities for a match if there's no top level match)
            return DefaultView(model.Item.ParentName);
        }

        public ActionResult CreateEffect(DynSingleView<Effect> model)
        {
            string parentName = model.Item.ParentName;
            if (string.IsNullOrEmpty(parentName)) return View();

            var parentItem = db.GetParseablebyName(parentName);
            if (parentItem == null) return View();

            //Get the list of effects that this will be added to
            var effectList = parentItem.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(List<Effect>)).GetValue<List<Effect>>(parentItem);

            //Add it and save
            effectList.Add(model.Item);
            db.Save(saveLocation);

            return DefaultView(model.Item.ParentName);
        }

        [HttpPost]
        public ActionResult NewEffectView(string parentName)
        {
            var item = new Effect() { ParentName = parentName };
            var model = new DynSingleView<Effect>(item);
            return View("CreateEffect", model);
        }
    }
}