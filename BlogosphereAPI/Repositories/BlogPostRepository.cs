using BlogosphereAPI.Data;
using BlogosphereAPI.Models.Domain;
using BlogosphereAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlogosphereAPI.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogosphereDbContext context;

        public BlogPostRepository(BlogosphereDbContext context)
        {
            this.context = context;
        }

        // Add a new blog post
        public async Task<BlogPostResponseDto> AddBlogPostAsync(BlogPost blogPost)
        {
            // Validate and ensure related Tags exist in the database
            var tagIds = blogPost.Tags.Select(t => t.Id).ToList();
            var existingTags = await context.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();

            // Clear and reassign tags to ensure correct tracking
            blogPost.Tags.Clear();
            foreach (var tag in existingTags)
            {
                blogPost.Tags.Add(tag);
            }

            // Add the BlogPost to the database
            await context.Blogs.AddAsync(blogPost);
            await context.SaveChangesAsync();

            // Return a DTO to avoid exposing the domain model
            return new BlogPostResponseDto
            {
                Id = blogPost.Id,
                Heading = blogPost.Heading,
                Content = blogPost.Content,
                ShortDescription = blogPost.ShortDescription,
                PageTitle = blogPost.PageTitle,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                Author = blogPost.Author,
                PublishedDate = blogPost.PublishedDate,
                Visible = blogPost.Visible,
                Tags = blogPost.Tags.Select(tag => new TagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                }).ToList()
            };
        }


        // Delete a blog post
        public async Task<BlogPostResponseDto?> DeleteBlogAsync(Guid id)
        {
            // Fetch the blog with related tags
            var existingBlog = await context.Blogs
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBlog != null)
            {
                // Remove the blog
                context.Blogs.Remove(existingBlog);
                await context.SaveChangesAsync();

                // Return a DTO to avoid circular references
                return new BlogPostResponseDto
                {
                    Id = existingBlog.Id,
                    Heading = existingBlog.Heading,
                    Content = existingBlog.Content,
                    ShortDescription = existingBlog.ShortDescription,
                    PageTitle = existingBlog.PageTitle,
                    FeaturedImageUrl = existingBlog.FeaturedImageUrl,
                    UrlHandle = existingBlog.UrlHandle,
                    Author = existingBlog.Author,
                    PublishedDate = existingBlog.PublishedDate,
                    Visible = existingBlog.Visible,
                    Tags = existingBlog.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        DisplayName = tag.DisplayName
                    }).ToList()
                };
            }

            // Return null if no blog is found
            return null;
        }


        // Get all blog posts (returning DTOs to avoid circular reference)
        public async Task<IEnumerable<BlogPostResponseDto>> GetAllBlogsAsync()
        {
            var blogPosts = await context.Blogs.Include(x => x.Tags).ToListAsync();

            // Project to DTOs to avoid circular reference
            var blogPostDtos = blogPosts.Select(bp => new BlogPostResponseDto
            {
                Id = bp.Id,
                Heading = bp.Heading,
                Content = bp.Content,
                ShortDescription = bp.ShortDescription,
                PageTitle = bp.PageTitle,
                FeaturedImageUrl = bp.FeaturedImageUrl,
                UrlHandle = bp.UrlHandle,
                Author = bp.Author,
                PublishedDate = bp.PublishedDate,
                Visible = bp.Visible,
                Tags = bp.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    DisplayName = t.DisplayName
                }).ToList()
            });

            return blogPostDtos;
        }

        // Get a blog post by URL handle
        public async Task<BlogPostResponseDto?> GetBlogByUrlhandleAsync(string urlhandle)
        {
            return await context.Blogs
                .Include(x => x.Tags) // Include related tags
                .Where(x => x.UrlHandle == urlhandle)
                .Select(blog => new BlogPostResponseDto
                {
                    Id = blog.Id,
                    Heading = blog.Heading,
                    Content = blog.Content,
                    ShortDescription = blog.ShortDescription,
                    PageTitle = blog.PageTitle,
                    FeaturedImageUrl = blog.FeaturedImageUrl,
                    UrlHandle = blog.UrlHandle,
                    Author = blog.Author,
                    PublishedDate = blog.PublishedDate,
                    Visible = blog.Visible,
                    Tags = blog.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        DisplayName = tag.DisplayName
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }


        // Get a blog post by ID
        public async Task<BlogPostResponseDto?> GetBlogPostByIdAsync(Guid id)
        {
            return await context.Blogs
                .Include(x => x.Tags) // Include related tags
                .Where(x => x.Id == id)
                .Select(blog => new BlogPostResponseDto
                {
                    Id = blog.Id,
                    Heading = blog.Heading,
                    Content = blog.Content,
                    ShortDescription = blog.ShortDescription,
                    PageTitle = blog.PageTitle,
                    FeaturedImageUrl = blog.FeaturedImageUrl,
                    UrlHandle = blog.UrlHandle,
                    Author = blog.Author,
                    PublishedDate = blog.PublishedDate,
                    Visible = blog.Visible,
                    Tags = blog.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        DisplayName = tag.DisplayName
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        // Update a blog post
        public async Task<BlogPostResponseDto?> UpdateBlogPostAsync(BlogPost blogPost)
        {
            var existingBlog = await context.Blogs
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlog != null)
            {
                // Update basic properties
                existingBlog.PageTitle = blogPost.PageTitle;
                existingBlog.Content = blogPost.Content;
                existingBlog.Author = blogPost.Author;
                existingBlog.PublishedDate = blogPost.PublishedDate;
                existingBlog.ShortDescription = blogPost.ShortDescription;
                existingBlog.UrlHandle = blogPost.UrlHandle;
                existingBlog.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlog.Visible = blogPost.Visible;
                existingBlog.Heading = blogPost.Heading;

                // Update Tags
                // Clear existing tags, but ensure EF Core tracks changes
                existingBlog.Tags.Clear();
                foreach (var tag in blogPost.Tags)
                {
                    var existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tag.Id);
                    if (existingTag != null)
                    {
                        existingBlog.Tags.Add(existingTag); // Use the existing tag from the database
                    }
                    else
                    {
                        existingBlog.Tags.Add(new Tag
                        {
                            Id = tag.Id,
                            Name = tag.Name,
                            DisplayName = tag.DisplayName
                        });
                    }
                }

                // Save changes
                await context.SaveChangesAsync();

                // Project to DTO and return
                return new BlogPostResponseDto
                {
                    Id = existingBlog.Id,
                    Heading = existingBlog.Heading,
                    Content = existingBlog.Content,
                    ShortDescription = existingBlog.ShortDescription,
                    PageTitle = existingBlog.PageTitle,
                    FeaturedImageUrl = existingBlog.FeaturedImageUrl,
                    UrlHandle = existingBlog.UrlHandle,
                    Author = existingBlog.Author,
                    PublishedDate = existingBlog.PublishedDate,
                    Visible = existingBlog.Visible,
                    Tags = existingBlog.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        DisplayName = tag.DisplayName
                    }).ToList()
                };
            }

            return null;
        }

    }
}
