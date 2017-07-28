using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotA;
using DotA.Model;
using DotA.WebEdit.Models;
using DotA.WebEdit.Helpers;

namespace Data_Editor.Controllers
{
    public class HeroController : Controller
    {
        /// <summary>
        /// Store the data in a persistent session
        /// </summary>
        const string dataLocation = @"c:\users\dkaschel\desktop\dota.dat";
        const string dataInd = "data";
        public DotaData db => (DotaData)(Session[dataInd] == null ? (Session[dataInd] = DotaData.Load(dataLocation)) 
                                                                  : Session[dataInd]);

        // GET: Hero
        public ActionResult Index() => View("Heroes", new DynMultiView<Hero>(db.Heroes));

        public ActionResult Hero(string heroName) => View("HeroView", new DynSingleView<Hero>(db.Heroes.FirstOrDefault(h => h.Name == heroName) ??
                                                                                              db.Heroes.FirstOrDefault(h => h.Abilities.Any(a => a.Name == heroName))));
        

        public ActionResult Image(string dir, string filename)
        {
            var path = Path.Combine(dir, filename);
            return File(path, "image/png");
        }

        public ActionResult HeroUpdate(DynSingleView<Hero> model) => ItemUpdate(model);
        public ActionResult AbilityUpdate(DynSingleView<Ability> model) => ItemUpdate(model);

        public ActionResult ItemUpdate<T>(DynSingleView<T> model) where T: Parseable
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

            //refresh the whole hero page (the view will check the hero abilities for a match once
            //it realizes there's no hero by that name)
            return Hero(model.Item.Name);
        }

        public ActionResult Test()
        {
            return View("Test", new DynSingleView<Ability>(db.Abilities.Skip(5).FirstOrDefault()));
        }
    }
}