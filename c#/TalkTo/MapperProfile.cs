using AutoMapper;
using TalkTo.Models;

namespace TalkTo
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
            CreateMap<User, UserDTO>();
            CreateMap<Message, MessageDTO>();
        }
    }
}