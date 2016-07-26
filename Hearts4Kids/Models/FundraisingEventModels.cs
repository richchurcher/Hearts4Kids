using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hearts4Kids.Models
{
    public class FundraisingEventModel
    {
        public int? EventId { get; set; }
        [Display(Name="Event Name"), StringLength(50)]
        public string Name { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        [RegularExpression(@"^([0-9]|[0-1][0-9]|2[0-3]):[0-5][0-9]( ?[aApP]\.?[mM]?\.?)?$", ErrorMessage = "Invalid Time.")]
        [Required, DataType(DataType.Time)]
        public string Time { get; set; }
        [StringLength(256),Required]
        public string Location { get; set; }
        [StringLength(256),Display(Name="Flyer")]
        public string FlyerUrl { get; set; }
        [Required, Display(Name="Principal Organiser")]
        public int? PrincipalOrganiserId { get; set; }
        [DataType(DataType.MultilineText), Required, AllowHtml]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Organisers { get; set; }

        public DateTime EventDateTime
        {
            get
            {
                return Date.Value + DateTime.Parse(Time).TimeOfDay;
            }
            set
            {
                this.Date = value.Date;
                this.Time = value.ToString("hh:mm tt");
            }
        }
    }

    public class FundraisingEventDetails
    {
        public int Id { get; set; }
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