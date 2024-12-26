using BlogosphereAPI.Models.Domain;

namespace BlogosphereAPI.Repositories
{
    public interface IBlogPostLikeRepository
    {
        Task<int> GetAllLikes(Guid blogPostId);
        Task<BlogPostLike> AddOrRemoveBlogLike(BlogPostLike blogPostLike);
    }
}
