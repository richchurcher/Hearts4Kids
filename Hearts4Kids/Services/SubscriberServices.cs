using System;
using System.Linq;
using Hearts4Kids.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net.Mail;
using Hearts4Kids.Domain;
using static Hearts4Kids.Domain.DomainConstants;
using System.Text;
using System.Web;

namespace Hearts4Kids.Services
{
    public class SubscriberServices
    {
        public enum SubscribeResult { AlreadySubscribed, ReSubscribed, NotApplicableToTeamMembers, Success }
        public static async Task<bool> Unsubscribe(int subscriberId, Guid unsubscribeToken, SubscriptionTypes subscribeType)
        {
            using (var db = new Hearts4KidsEntities())
            {
                var a = await db.NewsletterSubscribers.FirstOrDefaultAsync(s => s.Id == subscriberId && s.UnsubscribeToken == unsubscribeToken);
                if (a == null)
                {
                    return false;
                }
                a.Subscription = subscribeType;
                await db.SaveChangesAsync();
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribe"></param>
        /// <param name="getLink">If null, no email is sent</param>
        /// <returns></returns>
        public static async Task<SubscribeResult> AddEmail(SubscribeModel subscribe, bool sendConfirmation)
        {
            //unecessary - validate model state before getting to here
            //if (!new RegexUtilities().IsValidEmail(email))
            using (var db = new Hearts4KidsEntities())
            {
                //options - is a user - ignore, in DB but unsubscribed, not in DB
                if (await db.AspNetUsers.AnyAsync(u => u.Email == subscribe.Email)) { return SubscribeResult.NotApplicableToTeamMembers; }
                var sub = await (from s in db.NewsletterSubscribers
                                    where s.Email == subscribe.Email
                                    select new { unsub = s.Subscription, id=s.Id }).FirstOrDefaultAsync();
                if (sub == null)
                {
                    var newSubscriber = new NewsletterSubscriber
                    {
                        Email = subscribe.Email,
                        Subscription = subscribe.subscribeType(),
                        UnsubscribeToken =Guid.NewGuid()
                    };
                    db.NewsletterSubscribers.Add(newSubscriber);
                    await db.SaveChangesAsync();
                    if (sendConfirmation) { await SendEmailWelcome(subscribe.Email, newSubscriber.Id, newSubscriber.UnsubscribeToken, newSubscriber.Subscription, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)); }
                    return SubscribeResult.Success;
                }
                else
                {
                    var subscriber = new NewsletterSubscriber { Id = sub.id, Subscription = subscribe.subscribeType(), Email=subscribe.Email }; //email must not be null to avid entity validation error
                    db.NewsletterSubscribers.Attach(subscriber);
                    db.Entry(subscriber).Property(s => s.Subscription).IsModified = true;
                    await db.SaveChangesAsync();
                    
                    if (sendConfirmation) { await SendEmailWelcome(subscribe.Email, subscriber.Id, subscriber.UnsubscribeToken, subscribe.subscribeType(), HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)); }
                    return SubscribeResult.ReSubscribed;
                }
            }
        }

        public static async Task<IEnumerable<string>> GetEmails(string startsWith)
        {
            using (var db = new Hearts4KidsEntities())
            {
                var subscribers = await (from n in db.NewsletterSubscribers
                                         where n.Email.StartsWith(startsWith)
                                         select n.Email).ToListAsync();
                subscribers.AddRange(await (from u in db.AspNetUsers
                                            where u.Email.StartsWith(startsWith)
                                            select u.Email).ToListAsync());
                subscribers.Sort();
                return subscribers;
            }
        }

        public static async Task<DonorInfoSummaryModel> GetDonorInfo(string email)
        {
            using (var db = new Hearts4KidsEntities())
            {
                var donor = await (from n in db.NewsletterSubscribers
                                         where n.Email == email
                                         let corp = n.CorporateSponsor != null
                                         select new DonorInfoSummaryModel
                                         {
                                             Name = n.Name,
                                             Address =n.Address,
                                             CountryId =n.Country,
                                             IsOrganisation = corp,
                                             LogoSrc = corp?n.CorporateSponsor.LogoUrl:null,
                                             WebUrl = corp?n.CorporateSponsor.WebUrl:null,
                                             ExistingReceipts = n.Receipts.Select(r=>new PriorReceiptModel { Amount=r.Amount, DateReceived=r.DateReceived })
                                         }).FirstOrDefaultAsync();
                if (donor == null)
                {
                    donor = await (from u in db.AspNetUsers
                                   where u.Email == email
                                   let bio = u.UserBio
                                   select new DonorInfoSummaryModel
                                   {
                                       Name = (bio==null)?null:(bio.FirstName + " " + bio.Surname),
                                       Address = (bio==null)?null:bio.Address,
                                       ExistingReceipts = u.Receipts.Select(r => new PriorReceiptModel { Amount = r.Amount, DateReceived = r.DateReceived })
                                   }).FirstOrDefaultAsync();
                }
                return donor;
            }
        }

        static string CreateLink(string baseUrl,int subscriberId, Guid code, SubscriptionTypes currentSubscription)
        {
            return string.Format("{0}/Subscription/Unsubscribe?subscriberId={1}&code={2}&subscribeType={3}",
                baseUrl, subscriberId, code, currentSubscription);
        }
        public static string getUnsubscribeDetails(int subscriberId, Guid code,SubscriptionTypes subscribeType, string baseUrl)
        {
            if (subscribeType == SubscriptionTypes.NoEmails)
            {
                throw new Exception("Should not be sending emails to unsubscribed users");
            }
            StringBuilder returnVar = new StringBuilder("<p>If you would like more regular updates, you can follow us on <a href='http://www.facebook.com/Hearts4KidsPasifika'>Facebook</a> and <a href='https://twitter.com/Hearts4KidsNZ'>Twitter</a>.</p>"
                + "<p>If on the other hand, you would ever like to alter your email subscritpion:</p><dl>"
                + "<dt>Newsletter</dt><dd>"
                + "<p>We email updates a couple of times a year - on how the project is going, and how the children are doing.</p>"
                + "<p>you are currently ");
            if ((subscribeType & SubscriptionTypes.ReceiveNewsletter) == SubscriptionTypes.NoEmails)
            {
                returnVar.AppendFormat("unsubscribed from these newsletters. To <a href='{0}'>resubscribe, please click here</a>.",
                    CreateLink(baseUrl,subscriberId, code, subscribeType | SubscriptionTypes.ReceiveNewsletter));
            }
            else
            {
                returnVar.AppendFormat("subscribed to these newsletters. To <a href='{0}'>unsubscribe, please click here</a>.",
                    CreateLink(baseUrl, subscriberId, code, subscribeType & ~SubscriptionTypes.ReceiveNewsletter));
            }
            returnVar.Append("</p></dd><dt>Fundraising Events</dt><dd><p>We will let you know dates, times and locations for fundraising events."
                +"<p>You are currently ");
            if ((subscribeType & SubscriptionTypes.ReceiveFundraisers) == SubscriptionTypes.NoEmails)
            {
                returnVar.AppendFormat("unsubscribed from these fundraising updates. To <a href='{0}'>resubscribe, please click here</a>.",
                    CreateLink(baseUrl, subscriberId, code, subscribeType | SubscriptionTypes.ReceiveFundraisers));
            }
            else
            {
                returnVar.AppendFormat("subscribed to these fundraising updates. To <a href='{0}'>unsubscribe, please click here</a>.",
                    CreateLink(baseUrl, subscriberId, code, subscribeType & ~SubscriptionTypes.ReceiveFundraisers));
            }
            returnVar.Append("</p></dd>");
            if (subscribeType == SubscriptionTypes.FullSubscription)
            {
                returnVar.AppendFormat("<dt>Unsubscribe from all emails</dt><dd><p><a href='{0}'>Click here if you wish to unsubscribe</a> from <em>any and all emails</em> from Hearts4Kids.</p></dd>",
                    CreateLink(baseUrl, subscriberId, code, SubscriptionTypes.NoEmails));
            }
            returnVar.Append("</dl><p>To provide us with any feedback, you can reply to this email.<p>");
            return returnVar.ToString();
        }
        public static async Task SendEmailWelcome(string email, int subscriberId, Guid code, SubscriptionTypes subscribeType, string baseUrl)
        {
            var client = new SmtpClient();
            client.SendCompleted += (s, e) => {
                client.Dispose();
            };
            var m = new MailMessage {
                Subject = "Thank you for subscribing to Hearts4Kids - caring for the hearts of the pacific",
                Body = getUnsubscribeDetails(subscriberId, code, subscribeType, baseUrl),
                IsBodyHtml = true
            };
            m.To.Add(email);
            await client.SendMailAsync(m);
        }

        public static async Task SendSubscriberEmails(string subject, string message, params HttpPostedFileBase[] files)
        {
            var san = new Ganss.XSS.HtmlSanitizer();
            message = san.Sanitize(message);
            string usrs;
            IEnumerable<NewsletterSubscriber> subscribers;
            using (var db = new Hearts4KidsEntities())
            {
                usrs = string.Join(",",(from u in db.AspNetUsers
                                        select u.Email));
                subscribers = await (from s in db.NewsletterSubscribers
                                     where ((s.Subscription & SubscriptionTypes.ReceiveNewsletter) != SubscriptionTypes.NoEmails)
                                     select s).ToListAsync();
            }
            using (var client = new SmtpClient())
            {
                var m = new MailMessage
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                m.Bcc.Add(usrs);
                foreach(var f in files)
                {
                    var a = new Attachment(f.InputStream, f.ContentType);
                    a.Name = f.FileName;
                    m.Attachments.Add(a);
                }
                
                client.Send(m);
                m.Bcc.Clear();
                string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                foreach (var s in subscribers)
                {
                    m.To.Clear();
                    m.To.Add(s.Email);
                    m.Body = message + "<hr>" + getUnsubscribeDetails(s.Id, s.UnsubscribeToken, s.Subscription,baseUrl);
                    client.Send(m);
                }
            }
        }
    }
}