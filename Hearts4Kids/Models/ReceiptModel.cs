﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Hearts4Kids.Domain.DomainConstants;

namespace Hearts4Kids.Models
{
    public class DonorInfoModel
    {
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(256), DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Display(Name = "Country")]
        public Countries CountryId { get; set; }
        [Display(Name = "Is an Organisation")]
        public bool IsOrganisation { get; set; }
        [StringLength(128), Display(Name = "Home Page"), Url]
        public string WebUrl { get; set; }
        [StringLength(256), Display(Name = "Logo")]
        public string LogoSrc { get; set; }
    }
    public class ReceiptModel : DonorInfoModel
    {
        [EmailAddress, Required]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Date Received"), Required]
        public DateTime? DateReceived { get; set; }
        [Required, DataType(DataType.Currency), Range(0.05,1000000.0)]
        public decimal Amount { get; set; }
        [Required, Display(Name = "Type of transfer")]
        public DonationTypes? TransferMethodId { get; set; }
        [Display(Name = "Description", Description ="Not applicable to cash donations. Describe the goods, and in the case of a discount, also give details of the discount provided."),DataType(DataType.MultilineText), StringLength(2000)]
        public string Description { get; set; }
    }
}