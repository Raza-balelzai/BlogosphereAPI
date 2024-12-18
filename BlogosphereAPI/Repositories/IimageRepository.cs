namespace BlogosphereAPI.Repositories
{
    public interface IimageRepository
    {
        Task<string> UploadImageAsync(IFormFile img);
    }
}
