using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hearts4Kids.Models
{
    public class BiosViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Citation Description", Description = "If we quote you, how would you like to be referred to")]
        [StringLength(128)]
        public string CitationDescription { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(4000)]
        [AllowHtml]
        public string Biography { get; set; }

        [StringLength(256)]
        [Display(Name="Bio Picture",Description ="Picture to accompany your biography")]
        public string BioPicUrl { get; set; }
    }
}