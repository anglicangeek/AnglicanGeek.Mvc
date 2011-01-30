
namespace AnglicanGeek.Mvc
{
    public class RazorViewEngine : System.Web.Mvc.RazorViewEngine
    {
        public RazorViewEngine()
        {
            var areaViewLocations = new[] {  
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml",
                "~/Areas/{2}/Views/{0}.cshtml",
                "~/Areas/{2}/Views/{0}.vbhtml"
            };
            var viewLocations = new[] {  
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml",
                "~/Views/{0}.cshtml",
                "~/Views/{0}.vbhtml"
            };

            this.AreaMasterLocationFormats = areaViewLocations;
            this.AreaPartialViewLocationFormats = areaViewLocations;
            this.AreaViewLocationFormats = areaViewLocations;
            this.MasterLocationFormats = viewLocations;
            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;
        }
    }
}
