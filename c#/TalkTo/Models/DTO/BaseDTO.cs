using System.Collections.Generic;

namespace TalkTo.Models
{
    public class BaseDTO
    {
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}