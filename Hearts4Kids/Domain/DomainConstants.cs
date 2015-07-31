namespace Hearts4Kids.Domain
{
    public static class DomainConstants
    {
        public const int ReceiptIdentitySeed = 100000000;
        public const string Admin = "Admin";
        public enum Teams
        {
            Theatres,
            ICU,
            Wards,
            AllAreas,
            NewZealandAdmin,
            FijiSupportCrew,
            MajorFundraiser
        }
        public enum Professions
        {
            Perfusionist,
            Consultant,
            Registrar,
            AnaestheticTechnician,
            NurseSpecialist,
            Nurse,
            NonMedicalProfessional,
            Student
        }
        public enum DonationTypes
        {
            DirectBankTransfer = 1,
            GiveALittle = 2,
            GoodsInKind = 3,
            DiscountedGoods = 4,
            Cheque =5
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