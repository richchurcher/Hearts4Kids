using Hearts4Kids.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hearts4Kids.Controllers
{
    public class GalleryController : Controller
    {
        // GET: Gallery
        public ActionResult Index()
        {
            return View(PhotoServices.GetImages());
        }
        //[Authorize]
        public ActionResult MakeBanner()
        {
            return View(PhotoServices.GetImages());
        }
        /*[Authorize, */ [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MakeBanner(string[] forBanner)
        {
            PhotoServices.makeBanner(forBanner);
            return RedirectToAction("Index","Home");
        }
    }
}