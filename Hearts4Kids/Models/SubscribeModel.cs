using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Hearts4Kids.Domain.DomainConstants;

namespace Hearts4Kids.Models
{
    public class SubscribeModel
    {
        public SubscribeModel()
        {
            Newsletter = true;
            Fundraisers = true;
        }
        [EmailAddress, StringLength(128)]
        public string Email { get; set; }
        public bool Newsletter { get; set; }
        [Display(Name ="Fundraising Events")]
        public bool Fundraisers { get; set; }
    }
    public static class SubscribeExtensions
    {
        public static SubscriptionTypes subscribeType(this SubscribeModel model)
        {
            return (SubscriptionTypes)((model.Newsletter ? 1 : 0) | (model.Fundraisers ? 2 : 0));
        }
    }
    public class ChangeSubscriptionModel
    {
        public bool Success { get; set; }
        public SubscriptionTypes CurrentSubscription { get; set; }
    }
    public class SubscriberEmailDetails
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public Guid UnsubscribeGuid { get; set; }
        public SubscriptionTypes Subscription { get; set; }
    }
}