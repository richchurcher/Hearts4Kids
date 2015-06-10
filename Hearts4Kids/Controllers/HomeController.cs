using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hearts4Kids.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Team()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Donate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Subscribe(string subscriberMail)
        {
            
            if (!Services.SubscribeServices.AddEmail(subscriberMail))
            {
                //to do return error
            }
            return Content(string.Empty);
        }

        public ActionResult FAQ()
        {
            ViewBag.Message = "Your frequently asked Questions page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}