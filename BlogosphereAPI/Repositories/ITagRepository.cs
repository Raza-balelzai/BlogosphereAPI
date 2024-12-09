using BlogosphereAPI.Models.Domain;

namespace BlogosphereAPI.Repositories
{
    public interface ITagRepository
    {
        Task<Tag> AddTagAsync(Tag tag);
        Task<Tag> UpdateTagAsync(Tag tag,string tagName);
        Task<Tag> FindByNameAsync(string name);
        Task<Tag> DeleteTagAsync(string name);
        Task<IEnumerable<Tag>> GetAllTagsAsync();

    }
}
