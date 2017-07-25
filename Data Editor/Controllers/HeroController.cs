using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotA;
using DotA.Model;
using DotA.WebEdit.Models;

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

        public ActionResult Hero(string heroName) => View("HeroView", new DynSingleView<Hero>(db.Heroes.FirstOrDefault(h => h.Name == heroName)));

        public ActionResult Image(string dir, string filename)
        {
            var path = Path.Combine(dir, filename);
            return File(path, "image/png");
        }

        public ActionResult HeroUpdate(DynSingleView<Hero> model)
        {
            var hero = db.Heroes.First(h => model.Item.Name == h.Name);
            bool updated = false;
            foreach (var dv in model.DisplayValues.Where(dv => dv.Editable))
            {
                //see if anything has changed
                string newStrVal = dv.GetValue(model.Item);
                string oldStrVal = dv.GetValue(hero);
                if (newStrVal == oldStrVal) continue;

                //if it has changed, set the change
                updated = true;
                dv.SetValue(hero, newStrVal);
            }

            if (updated)
            {
                DotaData.Save(db, dataLocation);
                Session[dataInd] = null; //forces data to be refreshed
            }
            return Hero(model.Item.Name);
        }
    }
}