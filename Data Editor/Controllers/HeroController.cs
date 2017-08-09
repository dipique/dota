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

namespace DotA.WebEdit.Controllers
{
    public class HeroController : DotAController
    {
        public override ActionResult Index() => View("Heroes", new DynMultiView<Hero>(db.Heroes));

        public ActionResult Hero(string heroName) => View("HeroView", new DynSingleView<Hero>(db.Heroes.FirstOrDefault(h => h.Name == heroName) ??
                                                                                              db.Heroes.FirstOrDefault(h => h.Abilities.Any(a => a.Name == heroName))));
        public override ActionResult DefaultView(string itemName) => Hero(itemName);


        public ActionResult HeroUpdate(DynSingleView<Hero> model) => ItemUpdate(model);
        public ActionResult AbilityUpdate(DynSingleView<Ability> model) => ItemUpdate(model);
        public override ActionResult EffectUpdate(DynSingleView<Effect> model)
        {
            if (Request.Form.AllKeys.Contains("Delete"))
            {
                return DeleteEffect(model);
            }

            return base.EffectUpdate(model);
        }
            

        public ActionResult Test()
        {
            return View("Test", new DynSingleView<Ability>(db.Abilities.Skip(5).FirstOrDefault()));
        }
    }
}