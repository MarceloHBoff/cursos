using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalkTo.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmitterId { get; set; }
        [ForeignKey("EmitterId")]
        public User Emitter { get; set; }

        [Required]
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }

        [Required]
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
