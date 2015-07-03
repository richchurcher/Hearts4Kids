using Hearts4Kids.Domain;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Hearts4Kids.Models
{ 
    public class BiosViewModel :BioSumaryModel
    {
        [DataType(DataType.MultilineText)]
        [StringLength(4000)]
        [AllowHtml]
        public string Biography { get; set; }

        [StringLength(256)]
        [Display(Name="Bio Picture",Description ="Picture to accompany your biography")]
        public string BioPicUrl { get; set; }

        public bool IsAdmin { get; set; }
    }

    public class BioSumaryModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Citation Description", Description = "If we quote you, how would you like to be referred to")]
        [StringLength(128)]
        public string CitationDescription { get; set; }

        [Display(Name = "Team Leader Page")]
        public bool MainTeamPage { get; set; }
        public bool Approved { get; set; }

    }

    public class AllUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Admin")]
        public bool IsAdministrator { get; set; }
        public bool IsSelf { get; set; }
        [Display(Name = "Registered")]
        public bool HasRegistered { get; set; }
        public int BioLength { get; set; }
    }

    public class BioDisplay
    {
        public string Name { get; set; }
        public DomainConstants.Teams Team { get; set; }
        public DomainConstants.Professions Profession { get; set; }
        public string BioPicUrl { get; set; }
        public string Bio { get; set; }
        public bool Trustee { get; set; }
        public string CitationDescription { get; set; }
    }
}