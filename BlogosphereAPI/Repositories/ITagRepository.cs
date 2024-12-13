using BlogosphereAPI.Models.Domain;

namespace BlogosphereAPI.Repositories
{
    public interface ITagRepository
    {
        Task<Tag> AddTagAsync(Tag tag);
        Task<Tag> UpdateTagAsync(Tag tag,Guid id);
        Task<Tag> FindByNameAsync(string name);
        Task<Tag> GetByIdAsync(Guid id);
        Task<Tag> DeleteTagAsync(string id);
        Task<IEnumerable<Tag>> GetAllTagsAsync();

    }
}
