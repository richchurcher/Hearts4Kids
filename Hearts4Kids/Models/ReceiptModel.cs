using Mvc.JQuery.DataTables;
using System;
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
    public class DonorInfoSummaryModel : DonorInfoModel
    {
        public IEnumerable<PriorReceiptModel> ExistingReceipts { get; set; }
    }
    public class PriorReceiptModel
    {
        [DataType(DataType.Date)]
        public DateTime DateReceived { private get; set; }
        public String DateString { get { return DateReceived.ToShortDateString(); } }
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }

    public class DonorListItemModel
    {
        public int ReceiptNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [DataTablesFilter(DataTablesFilterType.DateRange), DataTables(DisplayName = "Date Received")]
        public DateTime DateReceived { get; set; }
        [DataTablesFilter(DataTablesFilterType.DateTimeRange), DataTables(DisplayName = "Receipt Issued")]
        public DateTime ReceiptDate { get; set; }
        //[DataTables(MRenderFunction = "asCurrency")]
        [DataTablesFilter(DataTablesFilterType.NumberRange)]
        public decimal Amount { get; set; }
        [DataTables(DisplayName = "Transfer")]
        public DonationTypes TransferMethod { get; set; }
        [DataTables(DisplayName = "Description")]
        public string Description { get; set; }
    }
}