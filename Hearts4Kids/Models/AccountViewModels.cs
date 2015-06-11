using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hearts4Kids.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class CreateUsersViewModel
    {
        [DataType(DataType.MultilineText)]
        [Display(Name = "List of Emails", Description = "Separated by semicolon(;) or comma(,)")]
        public string EmailList { get; set; }
        [Display(Name = "Make Administrators", Description = "Will apply to all emails provided")]
        public bool IsAdministrator { get; }
    }

    public class CreateUserViewModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(128)]
        public string Email { get; set; }
    }



    public class BioDetailsViewModel : CreateUserViewModel
    {
        public int UserId { get; set; }

        [Required, StringLength(128)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required, StringLength(128)]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required, StringLength(128)]
        [Display(Name = "Surame")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Profesional Role")]
        public Domain.Professions Profession { get; set; }

        [Required]
        [Display(Name = "Team")]
        public Domain.Teams Team { get; set; }

        [Required]
        [Display(Name = "Trustee", Description = "Check if you are you a nominated trustee for Hearts4Kids")]
        public bool Trustee { get; set; }

        [Display(Name = "Citation Description", Description = "If we quote you, how would you like to be referred to")]
        [StringLength(128)]
        public string CitationDescription { get; set; }

        [StringLength(128)]
        [Display(Name = "Phone Number", Description = "Not required, but will help your colleages contact you to discuss planning")]
        public string PhoneNumber { get; set; }
    }

    public class RegisterDetailsViewModel : BioDetailsViewModel
    { 

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
