
using System.ComponentModel.DataAnnotations;

namespace PertixCore.Resources
{
    public class UserLoginResource
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
