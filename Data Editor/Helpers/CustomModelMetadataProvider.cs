using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    public class CustomModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        //credits: http://odetocode.com/blogs/scott/archive/2012/09/04/working-with-enums-and-templates-in-asp-net-mvc.aspx
        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var result = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            if (result.TemplateHint == null &&
                typeof(Enum).IsAssignableFrom(result.ModelType))
            {
                result.TemplateHint = "Enum";
            }
            return result;
        }
    }
}