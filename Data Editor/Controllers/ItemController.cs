using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotA;
using DotA.Model;
using DotA.WebEdit.Models;
using DotA.WebEdit.Helpers;

namespace DotA.WebEdit.Controllers
{
    public class ItemController : DotAController
    {
        public override ActionResult Index() => View("Items", new DynMultiView<Item>(db.Items));

        public ActionResult Item(string itemName) => View("ItemView", new DynSingleView<Item>(db.Items.FirstOrDefault(h => h.Name == itemName) ??
                                                                                              db.Items.FirstOrDefault(h => h.Active.Name == itemName)));
        public override ActionResult DefaultView(string itemName) => Item(itemName);
    }
}
