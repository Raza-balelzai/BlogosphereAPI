using BlogosphereAPI.Models.Domain;
using BlogosphereAPI.Models.DTOs;
using BlogosphereAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BlogosphereAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminBlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IimageRepository imageRepository;
        private readonly ITagRepository tagRepository;

        public AdminBlogPostsController(IBlogPostRepository blogPostRepository,IimageRepository imageRepository, ITagRepository tagRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.imageRepository = imageRepository;
            this.tagRepository = tagRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlogPost([FromBody] AddBlogPostRequest addBlogPostRequest)
        {
            if (addBlogPostRequest == null)
            {
                return BadRequest(new { message = "Blog post data is required." });
            }

            // Map AddBlogPostRequest to Domain Model
            var blogPostDomainModel = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                Content = addBlogPostRequest.Content,
                PageTitle = addBlogPostRequest.PageTitle,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle.Replace(" ", "-").ToLower(),
                ShortDescription = addBlogPostRequest.ShortDescription,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
            };

            // Assign Tags to Domain Model
            if (addBlogPostRequest.SelectedTags != null)
            {
                var selectedTags = new List<Tag>();
                foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
                {
                    if (Guid.TryParse(selectedTagId, out var tagId))
                    {
                        var existingTag = await tagRepository.GetByIdAsync(tagId);
                        if (existingTag != null)
                        {
                            selectedTags.Add(existingTag);
                        }
                    }
                }
                blogPostDomainModel.Tags = selectedTags;
            }

            // Add Blog Post
            var createdBlogPost = await blogPostRepository.AddBlogPostAsync(blogPostDomainModel);

            if (createdBlogPost != null)
            {
                return Ok(new
                {
                    message="Blog Post Added Successfully."
                });
            }

            return Problem("An error occurred while creating the blog post.", null, (int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile img)
        {
            var imgURL = await imageRepository.UploadImageAsync(img);
            if (imgURL == null)
            {
                return Problem("Something went wrong while uploading the featured image.", null, (int)HttpStatusCode.InternalServerError);
            }
            return new JsonResult(new { LinkedList = imgURL });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await blogPostRepository.GetAllBlogsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "Blog ID is required." });
            }

            if (!Guid.TryParse(id, out var blogId))
            {
                return BadRequest(new { message = "Invalid blog ID format." });
            }

            var blog = await blogPostRepository.GetBlogPostByIdAsync(blogId);
            if (blog == null)
            {
                return NotFound(new { message = "No blog post found with the given ID." });
            }

            return Ok(blog);
        }

        [HttpGet("{urlHandle}")]
        public async Task<IActionResult> GetBlogByUrlHandle([FromRoute] string urlHandle)
        {
            if (string.IsNullOrWhiteSpace(urlHandle))
            {
                return BadRequest(new { message = "URL handle is required." });
            }

            var blogPost = await blogPostRepository.GetBlogByUrlhandleAsync(urlHandle.Replace(" ","-").ToLower());
            if (blogPost == null)
            {
                return NotFound(new { message = "No blog post found with the given URL handle." });
            }

            return Ok(blogPost);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "Blog ID is required." });
            }

            if (!Guid.TryParse(id, out var blogId))
            {
                return BadRequest(new { message = "Invalid blog ID format." });
            }

            var deletedBlog = await blogPostRepository.DeleteBlogAsync(blogId);
            if (deletedBlog == null)
            {
                return NotFound(new { message = "No blog post found with the given ID." });
            }

            return Ok(new { message = "Blog post deleted successfully.", item = deletedBlog.Heading });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogPost([FromBody] updateBlogPostDto updateBlogPostDto, [FromRoute] string id)
        {
            if (updateBlogPostDto == null || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "Blog post data and ID are required." });
            }

            if (!Guid.TryParse(id, out var blogId))
            {
                return BadRequest(new { message = "Invalid blog ID format." });
            }

            // Map UpdateBlogPostDto to Domain Model
            var blogPostDomainModel = new BlogPost
            {
                Id = blogId,
                Heading = updateBlogPostDto.Heading,
                UrlHandle = updateBlogPostDto.UrlHandle.Replace(" ", "-").ToLower(),
                Content = updateBlogPostDto.Content,
                PageTitle = updateBlogPostDto.PageTitle,
                FeaturedImageUrl = updateBlogPostDto.FeaturedImageUrl,
                ShortDescription = updateBlogPostDto.ShortDescription,
                PublishedDate = updateBlogPostDto.PublishedDate,
                Author = updateBlogPostDto.Author,
                Visible = updateBlogPostDto.Visible,
            };

            // Assign Tags to Domain Model
            if (updateBlogPostDto.SelectedTags != null)
            {
                var selectedTags = new List<Tag>();
                foreach (var selectedTag in updateBlogPostDto.SelectedTags)
                {
                    if (Guid.TryParse(selectedTag, out var tagId))
                    {
                        var existingTag = await tagRepository.GetByIdAsync(tagId);
                        if (existingTag != null)
                        {
                            selectedTags.Add(existingTag);
                        }
                    }
                }
                blogPostDomainModel.Tags = selectedTags;
            }

            // Update Blog Post
            var updatedBlogPost = await blogPostRepository.UpdateBlogPostAsync(blogPostDomainModel);

            if (updatedBlogPost != null)
            {
                return Ok(new
                {
                    message = "Blog post updated successfully.",
                    updatedBlogPost
                });
            }

            return BadRequest(new { message = "Unable to update the blog post." });
        }
    }
}
