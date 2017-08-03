using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotA.Model;

namespace DotA.WebEdit.Models
{
    public class CreateViewModel<T> where T : Parseable
    {
        public string ParentName { get; set; } //blank or null means it's a top-level entity like a hero or item

        public CreateViewModel(T test)
        {
        }

        //private DynSingleView
        //public DynSingleView<T> GetView<T>(T )
        //{

        //}
    }
}