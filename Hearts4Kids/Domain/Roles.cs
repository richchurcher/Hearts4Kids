using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hearts4Kids
{
    public static class Domain
    {
        public const string Admin = "Admin";
        public enum Teams
        {
            Theatres,
            ICU,
            Wards,
            AllAreas,
            NewZealand
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
    }
}