using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces;

public interface IUserProfilePictureRepository
{
    Task<bool> AddImage(ProfilePicture photo);
    Task<bool> Save();
}