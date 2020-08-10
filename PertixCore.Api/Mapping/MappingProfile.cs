using PertixCore.Resources;
using AutoMapper;
using PertixCore.Core.Models;

namespace PertixCore.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserSignUpResource, User>().ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
        }
    }
}