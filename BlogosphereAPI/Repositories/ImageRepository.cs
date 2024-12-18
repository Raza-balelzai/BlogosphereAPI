
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BlogosphereAPI.Repositories
{
    public class ImageRepository : IimageRepository
    {
        private readonly IConfiguration config;
        private readonly Account cloudinaryAccount;

        public ImageRepository(IConfiguration config)
        {
            cloudinaryAccount = new Account(
                config.GetSection("Cloudinary")["CloudName"],
                config.GetSection("Cloudinary")["ApiKey"],
                config.GetSection("Cloudinary")["ApiSecret"]
                );
            this.config = config;
        }
        public async Task<string> UploadImageAsync(IFormFile img)
        {
            var client=new Cloudinary(cloudinaryAccount);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(img.FileName,img.OpenReadStream()),
                DisplayName = img.Name,
            };
            var uploadResult =await client.UploadAsync(uploadParams);
            if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK) 
            {
                return uploadResult.SecureUrl.ToString();
            }
            return null;
        }
    }
}
