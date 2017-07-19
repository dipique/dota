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
        /// <summary>
        /// Store the data in a persistent session
        /// </summary>
        const string dataLocation = @"c:\users\dkaschel\desktop\dota.dat";
        const string dataInd = "data";
        public DotaData db => (DotaData)(Session[dataInd] == null ? (Session[dataInd] = DotaData.Load(dataLocation)) 
                                                                  : Session[dataInd]);

        // GET: Hero
        public ActionResult Index()
        {
            DotaData data = DotaData.Load(dataLocation);
            return View("Heroes", new DynMultiView<Hero>(db.Heroes));
        }

        public ActionResult Hero(string heroName)
        {
            DotaData data = DotaData.Load(dataLocation);
            return View("HeroView", new DynSingleView<Hero>(db.Heroes.FirstOrDefault(h => h.Name == heroName)));
        }
    }
}