﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Net;
using System.Net.Mail;

using Hearts4Kids.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Hearts4Kids.Services;

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

        public ActionResult Contact(ContactViewModel model)
        {
            ViewBag.Message = "Your contact page.";

            return View(model);
        }

        public ActionResult Success()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactSubmit(
            [Bind(Include = "FromName, FromEmail, FromPhone, Message")]
        ContactViewModel model)
        {
            try
            {
               if (ModelState.IsValid)
                {
                    const string body = "<p>Email From: {0} ({1}) Ph: {2}</p><p>Message:</p><p>{3}</p>";
                    var msg = new IdentityMessage
                    {
                        Body = string.Format(body, model.FromName, model.FromEmail, model.FromPhone, model.Message),
                        Subject = "H4K Web form Message"
                    };
                    await EmailService.SendEmailsToRoleAsync(Domain.Admin, msg);
                    return RedirectToAction("Success");
                }
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("", "Unable to send message. Please try again later.");
                model.Success = false;
                return View("Contact", model);
            }

            return View("Contact", model);
        }
    }
}