using AutoMapper;
using MinimalAPIConcepts.Dtos.UserDto;
using NEXT.GEN.Dtos.CommentDto;
using NEXT.GEN.Dtos.FriendshipDto;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Dtos.GroupMembersDto;
using NEXT.GEN.Dtos.LikeDto;
using NEXT.GEN.Dtos.Otp;
using NEXT.GEN.Dtos.PostDto;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Helper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<User, CreateUserDto>();
            CreateMap<CreateUserDto, User>();

            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();

            CreateMap<User, LoginUserDto>();
            CreateMap<LoginUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<GetUserDto, User>();

            // Friendship mapper
            CreateMap<Friends, CreateFriendshipDto>();
            CreateMap<CreateFriendshipDto, Friends>();

            // Post mapper
            CreateMap<CreatePost, CreatePostDto>();
            CreateMap<CreatePostDto, CreatePost>();
            CreateMap<GetUserDto, UpdateUserDto>();


            // Map UpdateUserDto to GetUserDto
            CreateMap<UpdateUserDto, GetUserDto>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Map GetUserDto to User entity
            CreateMap<GetUserDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<UpdateUserDto, User>();

            // add group mappers
            CreateMap<GetGroupMembersDTO, GroupMembers>();
            CreateMap<GroupMembers, GetGroupMembersDTO>();
            CreateMap<GroupMembers, JoinGroupDto>();
            CreateMap<JoinGroupDto, GroupMembers>();
            CreateMap<CreateGroupDto, Group>();
            CreateMap<Group, CreateGroupDto>();
            CreateMap<GetGroupDetailsDto, Group>();
            CreateMap<Group, GetGroupDetailsDto>();
            CreateMap<UploadPostToGroup, CreatePost>();
            CreateMap<CreatePost, UploadPostToGroup>();


            // group member and group
            CreateMap<GroupMembers, Group>();
            CreateMap<Group, GroupMembers>();


            // Like model
            CreateMap<Likes, AddLikeDto>();
            CreateMap<AddLikeDto, Likes>();


            // comment model
            CreateMap<Comment, CreateCommentDto>();
            CreateMap<CreateCommentDto, Comment>();
        }
    }
}