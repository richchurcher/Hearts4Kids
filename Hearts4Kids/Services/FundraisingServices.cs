using System;
using System.Collections.Generic;
using System.Linq;
using Hearts4Kids.Domain;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Hearts4Kids.Models;
using System.Web;
using static Hearts4Kids.Domain.DomainConstants;
using System.Web.Hosting;
using System.Threading;
using System.Net.Mail;
using System.Diagnostics;

namespace Hearts4Kids.Services
{
    public class FundraisingServices
    {
        public static async Task<IEnumerable<FundraisingEventDetails>> GetFundraisers()
        {
            using (var db = new Hearts4KidsEntities())
            {
                return await (from f in db.FundraisingEvents
                              where f.Date > DateTime.Now
                              orderby f.Date
                              let b = f.AspNetUser.UserBio
                              select new FundraisingEventDetails
                              {
                                  Id = f.Id,
                                  Date = f.Date,
                                  Description = f.Description,
                                  Location = f.Location,
                                  FlyerUrl = f.FlyerUrl,
                                  Name = f.Name,
                                  PrincipalOrganiser = b.FirstName + " " + b.Surname,
                                  PrincipalOrganiserId = f.AspNetUser.Id

                              }).ToListAsync();
            }
        }
        public static async Task<IEnumerable<SelectListItem>> GetMembers()
        {
            using (var db = new Hearts4KidsEntities())
            {
                return await MembersToSelectList(db).ToListAsync();
            }
        }
        static IQueryable<SelectListItem> MembersToSelectList(Hearts4KidsEntities db)
        {
            return (from u in db.UserBios
                    orderby u.FirstName, u.Surname
                    select new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.FirstName + " " + u.Surname
                    });
        }
        public static async Task<FundraisingEventModel> GetFundraiser(int id)
        {
            using (var db = new Hearts4KidsEntities())
            {
                var returnVar = await (from f in db.FundraisingEvents
                                  where f.Id == id
                                  select new FundraisingEventModel
                                  {
                                      EventId = f.Id,
                                      Description =f.Description,
                                      FlyerUrl =f.FlyerUrl,
                                      Location =f.Location,
                                      Name =f.Name,
                                      PrincipalOrganiserId = f.PrincipalOrganiserId,
                                      EventDateTime = f.Date
                                  }).FirstOrDefaultAsync();
                returnVar.Organisers = await MembersToSelectList(db).ToListAsync();
                return returnVar;
            }
        }
        const string flyerDir = "~/Content/Flyers/";
        public static async Task CreateFundraiser(FundraisingEventModel model, HttpPostedFileBase flyer)
        {
            var san = new Ganss.XSS.HtmlSanitizer();
            IEnumerable<string> userEmails;
            IEnumerable<SubscriberEmailDetails> subscriberEmails;
            string organiserName;
            FundraisingEvent e;
            using (var db = new Hearts4KidsEntities())
            {

                if (model.EventId.HasValue)
                {
                    e = db.FundraisingEvents.Find(model.EventId.Value);
                }
                else
                {
                    e = new FundraisingEvent();
                    db.FundraisingEvents.Add(e);
                }
                e.Date = model.EventDateTime;
                string oldPath = e.FlyerUrl;
                e.FlyerUrl = flyer == null ? null : (flyerDir + flyer.FileName);
                e.Location = model.Location;
                e.Name = model.Name;
                e.PrincipalOrganiserId = model.PrincipalOrganiserId.Value;
                e.Description = san.Sanitize(model.Description);//dont change baseUrl as it will be in email form

                await db.SaveChangesAsync();
                
                if (flyer != null)
                {
                    flyer.SaveAs(HostingEnvironment.MapPath(e.FlyerUrl));
                }
                if (!string.IsNullOrEmpty(oldPath) && oldPath != e.FlyerUrl)
                {
                    try
                    {
                        System.IO.File.Delete(HostingEnvironment.MapPath(oldPath));
                    }
                    catch (System.IO.IOException)
                    {
                    }
                    
                }
                if (model.EventId.HasValue) { return; }//everyone already emailed
                organiserName = (from b in db.UserBios
                                where b.Id == model.PrincipalOrganiserId
                                select b.FirstName + " " + b.Surname).FirstOrDefault();
                userEmails = await db.AspNetUsers.Select(u => u.Email).ToListAsync();
                subscriberEmails = (await (from s in db.NewsletterSubscribers
                                           where (s.Subscription & SubscriptionTypes.ReceiveFundraisers)!= 0
                                           select new SubscriberEmailDetails
                                           {
                                               Email = s.Email,
                                               Id = s.Id,
                                               UnsubscribeGuid =s.UnsubscribeToken,
                                               Subscription =s.Subscription
                                           }).ToListAsync());
            }
            string baseUr = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            Thread emailThread = new Thread(() => InviteAtendees(userEmails,subscriberEmails,baseUr,e, organiserName));
            emailThread.Start();
        }
        public static void InviteAtendees(IEnumerable<string> userEmails, IEnumerable<SubscriberEmailDetails> subscriberEmails, string baseUr,FundraisingEvent e, string organiserName)
        {
            var m = new MailMessage()
            {
                Subject = "Upcoming Hearts4Kids Event - " + e.Name,
                Body = "<table><tbody>"
                    + "<tr><td>Name</td><td>" + e.Name + "</td></tr>"
                    + "<tr><td>Location</td><td>" + e.Location + "</td></tr>"
                    + "<tr><td>Date</td><td>" + e.Date.ToString("f") + "</td></tr>"
                    + string.Format("<tr><td>Organiser</td><td><a href='{0}/Home/Contact/{1}'>{2}</a></td></tr>", baseUr,e.PrincipalOrganiserId, organiserName)
                    + "</tbody></table>"
                    +"<div>" + e.Description + "</div>"
                    + string.Format("<p>You can also find this information on the website at <a href='{0}/Fundraisers'>{0}/Fundraisers</a></p>",baseUr ),
                IsBodyHtml = true
            };

            m.To.Add(string.Join(",", userEmails));

            if (!string.IsNullOrEmpty(e.FlyerUrl))
            {
                var f = new Attachment(HostingEnvironment.MapPath(e.FlyerUrl));
                m.Attachments.Add(f);
            }


            using (var client = new SmtpClient())
            {
                client.Send(m);
                string baseBody = m.Body + "<hr/>";

                foreach (var s in subscriberEmails)
                {
                    m.To.Clear();
                    m.To.Add(s.Email);
                    m.Body = baseBody + SubscriberServices.getUnsubscribeDetails(s.Id, s.UnsubscribeGuid, s.Subscription,baseUr);
                    client.Send(m);
                }

            }
        }
    }
}