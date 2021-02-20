using System.ComponentModel.DataAnnotations;

namespace TalkTo.Models
{
    public class UserDTO : BaseDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }
        public string Slogan { get; set; }
    }
}
