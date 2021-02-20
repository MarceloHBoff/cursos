using Microsoft.AspNetCore.Identity;

namespace TalkTo.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Slogan { get; set; }
    }
}
