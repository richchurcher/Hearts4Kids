using System.ComponentModel.DataAnnotations;

namespace Hearts4Kids.Models
{
    public class ContactViewModel
    {
        public int? ContactId { get; set; }
        [Required, Display(Name = "Your name:"), StringLength(60,MinimumLength = 2)]
        public string FromName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Your email"), EmailAddress]
        public string FromEmail { get; set; }
        [DataType(DataType.PhoneNumber), RegularExpression(@"^[0-9\(\)\-\+ ]*$", ErrorMessage ="phone number must be a combination of digits(0-9), dashes(-), plus(+) and brackets'()'")]
        [Display(Name = "Your phone"), StringLength(20,MinimumLength =6)]
        public string FromPhone { get; set; }
        [Required]
        public string Message { get; set; }
        public bool? Success;
    }
    public class UserContacts
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}