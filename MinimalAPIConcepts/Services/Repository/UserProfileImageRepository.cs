using NEXT.GEN.Context;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository;

public class UserProfileImageRepository : IUserProfilePictureRepository
{
    private ApplicationDbContext _context;

    public UserProfileImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<bool> AddImage(ProfilePicture photo)
    {
        await _context.ProfilePictures.AddAsync(photo);
        return await Save();

    }

    public async Task<bool> Save()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}