using AutoMapper;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using NEXT.GEN.Dtos.FriendshipDto;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Dtos.GroupMembersDto;
using NEXT.GEN.Dtos.PostDto;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;

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
            CreateMap<User,GetUserDto>();
            CreateMap<GetUserDto,User>();

            // Friendship mapper
            CreateMap<Friendships,CreateFriendshipDto>();
            CreateMap<CreateFriendshipDto,Friendships>();

            // Post mapper
            CreateMap<CreatePost,CreatePostDto>();
            CreateMap<CreatePostDto,CreatePost>();

            // add group mappers
            CreateMap<GetGroupMembersDTO,GroupMembers>();
            CreateMap<GroupMembers,GetGroupMembersDTO>();
            CreateMap<GroupMembers,JoinGroupDto>();
            CreateMap<JoinGroupDto,GroupMembers>();
        }
    }
}
