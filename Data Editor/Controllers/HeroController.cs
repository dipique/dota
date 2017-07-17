using System;
using System.Collections.Generic;
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
        // GET: Hero
        public ActionResult Index()
        {
            DotaData data = DotaData.Load();
            return View("Heroes", new DynMultiView<Hero>(data.Heroes));
        }

        public ActionResult HeroView(string heroName)
        {
            DotaData data = DotaData.Load();
            return View("Hero", new DynSingleView<Hero>(data.Heroes.FirstOrDefault(h => h.Name == heroName)));
        }
    }
}