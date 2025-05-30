using NEXT.GEN.Dtos.UserDto.ProfileDtos;
using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces;

public interface ICloudinaryUploadRepository
{
    Task<ProfilePicture> UploadUserProfileToCloudinary(string userId, PhotoForCreationDto photoDto);
}