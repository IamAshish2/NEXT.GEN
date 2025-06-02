using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.UserDto.ProfileDtos;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotosController : ControllerBase
{
    private readonly IUserProfilePictureRepository _userProfilePictureRepository;
    private readonly ICloudinaryUploadRepository _cloudinaryUploadRepository;

    public PhotosController(IUserProfilePictureRepository userProfilePictureRepository, ICloudinaryUploadRepository cloudinaryUploadRepository)
    {
        _userProfilePictureRepository = userProfilePictureRepository;
        _cloudinaryUploadRepository = cloudinaryUploadRepository;
    }

    [HttpPost("add-profile-picture")]
    public async Task<IActionResult> AddProfilePicture(string userId, [FromForm] PhotoForCreationDto photoForCreationDto)
    {
        var profilePicture = await _cloudinaryUploadRepository.UploadUserProfileToCloudinary(userId, photoForCreationDto);
        
        var result = await _userProfilePictureRepository.AddImage(profilePicture);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest();
    }
}