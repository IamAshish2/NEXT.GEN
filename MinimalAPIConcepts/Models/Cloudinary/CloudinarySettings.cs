using Microsoft.Graph.TermStore;

namespace NEXT.GEN.Models.UserModel.Cloudinary;

public class CloudinarySettings
{
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecretKey { get; set; }
    public string UploadPreset { get; set; }
}