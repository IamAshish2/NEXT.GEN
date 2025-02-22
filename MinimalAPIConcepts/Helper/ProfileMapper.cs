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
            CreateMap<CreateUserDto,User>();
            CreateMap<User,UpdateUserDto>();
            CreateMap<UpdateUserDto,User>();
            CreateMap<User,LoginUserDto>();
            CreateMap<LoginUserDto,User>();
        }
    }
}
