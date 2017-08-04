using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

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
        internal const string dataLocation = @"c:\users\dkaschel\desktop\dota.dat";
        internal const string dataInd = "data";
        public DotaData db => (DotaData)(Session[dataInd] == null ? (Session[dataInd] = DotaData.Load(dataLocation))
                                                                  : Session[dataInd]);

        public ActionResult Image(string dir, string filename)
        {
            var path = Path.Combine(dir, filename);
            return File(path, "image/png");
        }

        public abstract ActionResult DefaultView(string itemName);
        public abstract ActionResult Index();

        public ActionResult ItemUpdate<T>(DynSingleView<T> model) where T : Parseable
        {
            //get the db list of with that object
            var allProps = typeof(DotaData).GetProperties();
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
                    string newStrVal = dv.GetValue(model.Item);
                    string oldStrVal = dv.GetValue(item);
                    if (newStrVal == oldStrVal) continue;

                    //if it has changed, set the change
                    updated = true;
                    dv.SetValue(item, newStrVal);
                }
            }

            if (updated)
            {
                DotaData.Save(db, dataLocation);
                Session[dataInd] = null; //forces data to be refreshed
            }

            //refresh the whole default page (the view will check the abilities for a match if there's no top level match)
            return DefaultView(model.Item.Name);
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
            DotaData.Save(db, dataLocation);

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