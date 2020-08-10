using System.ComponentModel.DataAnnotations;

namespace PertixCore.Api.Resources
{
    public class ForgotPasswordResource
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
