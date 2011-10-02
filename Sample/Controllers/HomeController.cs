using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(
            IGreetingService greetingService,
            string name)
        {
            ViewBag.Greeting = greetingService.GetGreeting(name);
            
            return View();
        }

        public ActionResult Fnord()
        {
            return View();
        }
    }
}
