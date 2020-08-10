using System.ComponentModel.DataAnnotations;

namespace PertixCore.Api.Resources
{
    public class ResetPasswordResource
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string NewPasswordConfirmation { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
