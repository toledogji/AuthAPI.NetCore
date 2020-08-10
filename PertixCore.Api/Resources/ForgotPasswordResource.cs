using System.ComponentModel.DataAnnotations;

namespace PertixCore.Api.Resources
{
    public class ForgotPasswordResource
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Compare("Email", ErrorMessage = "The email and confirmation password do not match.")]
        public string EmailConfirmation { get; set; }
    }
}
