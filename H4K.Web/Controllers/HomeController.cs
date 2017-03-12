using System.Diagnostics;
using System.Web.Mvc;
using H4K.Core.Services.Robots;

namespace H4K.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRobotsService _robotsService;

        public HomeController(IRobotsService robotsService)
        {
            _robotsService = robotsService;
        }

        [Route("", Name = "Index")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [Route("about", Name = "About")]
        public ActionResult About()
        {
            return View("About");
        }

        [Route("contact", Name = "Contact")]
        public ActionResult Contact()
        {
            return View("Contact");
        }

        [Route("robots.txt", Name = "robots.txt")]
        public ContentResult RobotsText()
        {
            Trace.WriteLine($"robots.txt requested. User Agent:<{this.Request.Headers.Get("User-Agent")}>.");
            return Content(_robotsService.GetRobotsText());
        }
    }
}