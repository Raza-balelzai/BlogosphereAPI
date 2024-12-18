using BlogosphereAPI.Models.Domain;
using BlogosphereAPI.Models.DTOs;

namespace BlogosphereAPI.Repositories
{
    public interface IBlogPostRepository
    {
        Task<BlogPostResponseDto> AddBlogPostAsync(BlogPost blogPost);
        Task<BlogPostResponseDto?> UpdateBlogPostAsync(BlogPost blogPost);
        Task<BlogPostResponseDto?> GetBlogByUrlhandleAsync(string urlhandle);
        Task<BlogPostResponseDto?> GetBlogPostByIdAsync(Guid id);
        Task<BlogPostResponseDto?> DeleteBlogAsync(Guid id);
        Task<IEnumerable<BlogPostResponseDto>> GetAllBlogsAsync();
    }
}
