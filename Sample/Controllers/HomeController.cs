using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        readonly IGreetingService greetingService;
        
        public HomeController(IGreetingService greetingService)
        {
            this.greetingService = greetingService;
        }
        
        public ActionResult Index()
        {
            ViewBag.Greeting = greetingService.GetGreeting();
            
            return View();
        }

        public ActionResult Fnord()
        {
            return View();
        }
    }
}
