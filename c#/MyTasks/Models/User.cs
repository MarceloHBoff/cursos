using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MyTasks.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Task> Tasks { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
