using BlogosphereAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogosphereAPI.Data
{
    public class BlogosphereDbContext:DbContext
    {
        public BlogosphereDbContext(DbContextOptions<BlogosphereDbContext> options):base(options) 
        {
            
        }
        public DbSet<BlogPost> Blogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostLike> BlogPostLikes { get; set; }
        public DbSet<BlogPostComment> BlogPostComments { get; set; }

    }
}
