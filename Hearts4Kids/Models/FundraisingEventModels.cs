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
        [Required, DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        [RegularExpression(@"^([0-9]|[0-1][0-9]|2[0-3]):[0-5][0-9] ?(am|pm|AM|PM)?$", ErrorMessage = "Invalid Time.")]
        [Required, DataType(DataType.Time)]
        public string Time { get; set; }
        [StringLength(256),Required]
        public string Location { get; set; }
        [StringLength(256),Display(Name="Flyer")]
        public string FlyerUrl { get; set; }
        [Required, Display(Name="Principal Organiser")]
        public int? PrincipalOrganiserId { get; set; }
        [DataType(DataType.MultilineText), StringLength(2000), Required, AllowHtml]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Organisers { get; set; }

        public DateTime EventDateTime
        {
            get
            {
                return Date.Value + DateTime.Parse(Time).TimeOfDay;
            }
        }
    }

    public class FundraisingEventDetails
    {
        [Display(Name = "Event Name"), StringLength(50)]
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime Date { get; set; }
        public string Location { get; set; }
        [Display(Name = "Flyer")]
        public string FlyerUrl { get; set; }
        [Display(Name = "Principal Organiser")]
        public string PrincipalOrganiser { get; set; }
        public int PrincipalOrganiserId { get; set; }
        public string Description { get; set; }
    }
}