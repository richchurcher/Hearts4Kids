using System.ComponentModel.DataAnnotations;

namespace Hearts4Kids.Models
{
    public class ContactViewModel
    {
        [Required, Display(Name = "Your name:")]
        public string FromName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Your email"), ]
        public string FromEmail { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Your phone")]
        public string FromPhone { get; set; }
        [Required]
        public string Message { get; set; }
        public bool? Success;
    }
}