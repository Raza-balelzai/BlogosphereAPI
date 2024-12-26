
using BlogosphereAPI.Data;
using BlogosphereAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogosphereAPI.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly BlogosphereDbContext blogosphereDbContext;

        public BlogPostLikeRepository(BlogosphereDbContext blogosphereDbContext)
        {
            this.blogosphereDbContext = blogosphereDbContext;
        }

        public async Task<BlogPostLike> AddOrRemoveBlogLike(BlogPostLike blogPostLike)
        {
            // Check if the like already exists
            var existingLike = await blogosphereDbContext.BlogPostLikes
                .FirstOrDefaultAsync(like => like.BlogPostId == blogPostLike.BlogPostId && like.UserId == blogPostLike.UserId);

            if (existingLike != null)
            {
                // If the like exists, remove it (unlike the post)
                blogosphereDbContext.BlogPostLikes.Remove(existingLike);
                await blogosphereDbContext.SaveChangesAsync();
                return null; // Return null to indicate the like was removed
            }
            else
            {
                // If the like doesn't exist, add it (like the post)
                await blogosphereDbContext.BlogPostLikes.AddAsync(blogPostLike);
                await blogosphereDbContext.SaveChangesAsync();
                return blogPostLike; // Return the added like
            }
        }


        public async Task<int> GetAllLikes(Guid blogPostId)
        {
           return await blogosphereDbContext.BlogPostLikes.CountAsync(x=>x.BlogPostId == blogPostId);
        }
    }
}
