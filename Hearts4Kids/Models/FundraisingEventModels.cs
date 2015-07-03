using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hearts4Kids.Models
{
    public class FundraisingEventModel
    {
        [Display(Name="Event Name"), StringLength(50)]
        public string Name { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        [StringLength(256),Required]
        public string Location { get; set; }
        [StringLength(256),Required, Display(Name="Flyer")]
        public string FlyerUrl { get; set; }
        [Required, Display(Name="Pricipal Organiser")]
        public int? PrincipalOrganiserId { get; set; }
        [DataType(DataType.MultilineText), StringLength(3000), Required, AllowHtml]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Organisers { get; set; }
    }
}