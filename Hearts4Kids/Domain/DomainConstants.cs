using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hearts4Kids.Domain
{
    public static class DomainConstants
    {
        public const string Admin = "Admin";
        public enum Teams
        {
            Theatres,
            ICU,
            Wards,
            AllAreas,
            NewZealand,
            Fiji
        }
        public enum Professions
        {
            Perfusionist,
            Consultant,
            Registrar,
            AnaestheticTechnician,
            NurseSpecialist,
            Nurse,
            NonMedical
        }
        public enum DonationTypes
        {
            DirectBankTransfer = 1,
            GiveALittle = 2,
            GoodsInKind = 3,
            DiscountedGoods = 4
        }
        public enum Countries
        {
            Australia = 2,
            Fiji = 4,
            NewZealand = 1,
            Other = 100,
            Unknown = 0,
            USA = 3
        }
        public enum SubscriptionTypes
        {
            
            NoEmails = 0,
            ReceiveNewsletter = 1,
            ReceiveFundraisers = 2,
            FullSubscription = 3,
        }
    }
}