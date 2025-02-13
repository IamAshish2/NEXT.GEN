using AutoMapper;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;

namespace MinimalAPIConcepts.Helper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<User,CreateUserDto>();
            CreateMap<User,UpdateUserDto>();
            CreateMap<User,LoginUserDto>();
        }
    }
}
