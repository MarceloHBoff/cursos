using AutoMapper;
using Mimic.Helpers;
using Mimic.Models;
using Mimic.Models.DTO;

namespace Mimic
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Word, WordDTO>();
            CreateMap<PaginationList<Word>, PaginationList<WordDTO>>();
        }
    }
}