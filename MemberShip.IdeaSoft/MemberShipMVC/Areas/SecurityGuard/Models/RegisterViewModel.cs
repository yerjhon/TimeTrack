using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MemberShipMVC.Areas.SecurityGuard.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "invalid character")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Secret Question")]
        public string SecretQuestion { get; set; }

        [Display(Name = "Secret Answer")]
        public string SecretAnswer { get; set; }

        public bool Approve { get; set; }

        public bool RequireSecretQuestionAndAnswer { get; set; }
    }
}
