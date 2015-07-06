using Hearts4Kids.Domain;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Hearts4Kids.Tests
{
    public class UpdateMembers
    {
        public void AddOrUpdateEmails()
        {
            string stringList = "'HSargent@adhb.govt.nz'; 'farmer_katherine@yahoo.com.au'; 'alex@mercerpalmer.com'; 'geoff@pownall.kiwi'; 'KFinucane@adhb.govt.nz'; 'MGreaves@adhb.govt.nz'; 'MMcGivern@adhb.govt.nz'; 'AnaK@adhb.govt.nz'; 'CRobinson@adhb.govt.nz'; 'DarrenR@adhb.govt.nz'; 'JackieO@adhb.govt.nz'; 'JohnWr@adhb.govt.nz'; 'keeley.lawson@gmail.com'; 'carolanne@slingshot.co.nz'; 'chrissieorchard@hotmail.com'; 'ruby.jess@hotmail.co.uk'; 'NCulliford@adhb.govt.nz'; 'MHamer@adhb.govt.nz'; 'meh@xtra.co.nz'; 'AStraaten@adhb.govt.nz'; 'KendyllB@adhb.govt.nz'; 'ABerghan@adhb.govt.nz'; 'ALiley@adhb.govt.nz'; 'BrentM@adhb.govt.nz'; 'DavidB@adhb.govt.nz'; 'sargents@xtra.co.nz'; 'ThomasH@adhb.govt.nz'; 'AnnD@adhb.govt.nz'; 'athompson333@gmail.com'; 'gaelmorrison@hotmail.com'; 'HBarr@adhb.govt.nz'; 'Lawlor@adhb.govt.nz'; 'YvonneV@adhb.govt.nz'; 'ShelleyBa@adhb.govt.nz'; 'YArcher@adhb.govt.nz'; 'JudithT@adhb.govt.nz'; 'VHollier@adhb.govt.nz'";
            var emailList = stringList.Split(new char[] { '\'', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<NewsletterSubscriber> newbies = new List<NewsletterSubscriber>();
            using (var db = new Hearts4KidsEntities())
            {
                foreach (var e in emailList)
                {
                    if (!db.AspNetUsers.Any(u=>u.Email==e) && !db.NewsletterSubscribers.Any(n => n.Email == e))
                    {
                        newbies.Add(new NewsletterSubscriber { Email = e, Country= DomainConstants.Countries.Unknown, Subscription= DomainConstants.SubscriptionTypes.FullSubscription, UnsubscribeToken = Guid.NewGuid() });
                    }
                }
            }

            Console.WriteLine("INSERT INTO [dbo].[NewsletterSubscribers] ([Email] ,[CountryId],[UnsubscribeToken],[SubscriptionTypeId]) VALUES "
                + string.Join(",",newbies.Select(n=> string.Format("({0},{1},{2},{3})",n.Email, (int)n.Country, n.UnsubscribeToken, (int)n.Subscription))));
        }
    }
}
