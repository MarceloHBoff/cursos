using System;

namespace TalkTo.Models
{
    public class MessageDTO : BaseDTO
    {
        public int Id { get; set; }
        public string EmitterId { get; set; }
        public User Emitter { get; set; }
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
