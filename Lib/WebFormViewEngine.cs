using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnglicanGeek.Mvc
{
    public class WebFormViewEngine : System.Web.Mvc.WebFormViewEngine
    {
        public WebFormViewEngine()
        {
            var areaMasterLocations = new[] {  
                "~/Areas/{2}/Views/{1}/{0}.master",
                "~/Areas/{2}/Views/Shared/{0}.master"
            };
            var areaViewLocations = new[] {  
                "~/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Areas/{2}/Views/Shared/{0}.aspx",
                "~/Areas/{2}/Views/Shared/{0}.ascx",
                "~/Areas/{2}/Views/{0}.aspx",
                "~/Areas/{2}/Views/{0}.ascx"
            };
            var masterLocations = new[] {  
                "~/Views/{1}/{0}.master",
                "~/Views/Shared/{0}.master"
            };
            var viewLocations = new[] {  
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.aspx",
                "~/Views/Shared/{0}.ascx",
                "~/Views/{0}.aspx",
                "~/Views/{0}.ascx"
            };

            this.AreaMasterLocationFormats = areaMasterLocations;
            this.AreaPartialViewLocationFormats = areaViewLocations;
            this.AreaViewLocationFormats = areaViewLocations;
            this.MasterLocationFormats = masterLocations;
            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;
        }
    }
}
