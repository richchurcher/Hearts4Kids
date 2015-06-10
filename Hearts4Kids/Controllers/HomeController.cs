using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Net;
using System.Net.Mail;

using Hearts4Kids.Models;
using System.Threading.Tasks;

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
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress("recipient@gmail.com"));  // replace with valid value 
                    message.Subject = "H4K Web form Message";
                    message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
/* Only for testing -- SetPasswordViewModel ICredentials in WebClient.config mailSettings (template already inserted) for the host site */
                        message.To.Add(new MailAddress("philm@codesmiths.com.au"));  // replace with valid value 
                        message.From = new MailAddress("philm@codesmiths.com.au");   // replace with valid value
                        var credential = new NetworkCredential
                        {
                            UserName = "philm@codesmiths.com.au",  // replace with valid value
                            Password = "76mangoes"                   // replace with valid value
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "mail.codesmiths.com.au";
                        smtp.Port = 25;
                        smtp.EnableSsl = false;
/* end testing block */
                        await smtp.SendMailAsync(message);
                        model.Success = true;
                        return View("Contact", model);
                    }                    
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