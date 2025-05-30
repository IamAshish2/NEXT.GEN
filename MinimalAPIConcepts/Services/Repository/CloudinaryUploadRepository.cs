using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using NEXT.GEN.Dtos.UserDto.ProfileDtos;
using NEXT.GEN.Models;
using NEXT.GEN.Models.Cloudinary;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository;

// handles uploading image to cloudinary
public class CloudinaryUploadRepository : ICloudinaryUploadRepository
{
    public IConfiguration Configuration { get; }
    private CloudinarySettings _cloudinarySettings;
    private Cloudinary _cloudinary;

    public CloudinaryUploadRepository(IConfiguration configuration)
    {
        Configuration = configuration;
        _cloudinarySettings = Configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
        Account account = new Account(
            _cloudinarySettings.CloudName,
            _cloudinarySettings.ApiKey,
            _cloudinarySettings.ApiSecretKey
        );
        _cloudinary = new Cloudinary(account);
    }


    public async Task<ProfilePicture> UploadUserProfileToCloudinary(string userId, PhotoForCreationDto photoDto)
    {
        var file = photoDto.File;
        
        var uploadResult = new ImageUploadResult();
        
        if (file.Length > 0)
        {
            var stream = file.OpenReadStream();
        
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream)
            };
        
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        // var uploadResult = await UploadToCloudinary(photoDto);
        photoDto.Url = uploadResult.Url.ToString();
        photoDto.PublicId = uploadResult.PublicId.ToString();

        var photo = new ProfilePicture()
        {
            Url = photoDto.Url,
            Description = "",
            DateAdded = photoDto.DateAdded,
            PublicId = photoDto.PublicId,
            UserId = userId
        };

        photo.IsMain = true;
        return photo;
    }

    private async Task<ImageUploadResult> UploadToCloudinary(PhotoForCreationDto photoDto)
    {
        var file = photoDto.File;

        if (file.Length < 1)
        {
            throw new ArgumentException("not found");
        }
        
        // returns ImageUploadResult i.e. the details of the uploaded file
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            // read the file
            var stream = file.OpenReadStream();
            
            // prepare the upload parameter required for cloudinary
            var uploadParams = new ImageUploadParams()
            {
                // insert the fileName along with the read file data
                File = new FileDescription(file.Name, stream)
            };
            
            // upload to cloudinary
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }
}