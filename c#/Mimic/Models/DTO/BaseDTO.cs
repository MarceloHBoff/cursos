using System.Collections.Generic;

namespace Mimic.Models.DTO
{
    public class BaseDTO
    {
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}