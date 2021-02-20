using System;

namespace Mimic.Models.DTO
{
    public class WordDTO : BaseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}