using BlogosphereAPI.Models.Domain;
using BlogosphereAPI.Models.DTOs;
using BlogosphereAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogosphereAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeCommentController : ControllerBase
    {
        private readonly IBlogPostLikeRepository blogPostLikeRepository;

        public LikeCommentController(IBlogPostLikeRepository blogPostLikeRepository)
        {
            this.blogPostLikeRepository = blogPostLikeRepository;
        }
        [HttpPost]
        public async Task<IActionResult> AddLike([FromBody] LikeCommentDTO likeCommentDTO)
        {
            // Validate UserId and BlogPostID
            var isValidUserId = Guid.TryParse(likeCommentDTO.UserId, out Guid userId);
            var isValidPostId = Guid.TryParse(likeCommentDTO.BlogPostID, out Guid postId);

            if (!isValidUserId || !isValidPostId)
            {
                return BadRequest(new { message = "Post or User ID is incorrect or empty." });
            }

            // Create BlogPostLike object
            var blogPostLike = new BlogPostLike
            {
                UserId = userId,
                BlogPostId = postId,
            };

            // Call repository method to add or remove the like
            var result = await blogPostLikeRepository.AddOrRemoveBlogLike(blogPostLike);

            // Respond based on the result
            if (result == null)
            {
                return Ok(new { message = "Like removed successfully." });
            }
            else
            {
                return Ok(new { message = "Like added successfully.", like = result });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPostLikes(string PostId)
        {
            bool parsedId=Guid.TryParse(PostId, out Guid guidPostId);
            if (!parsedId)
            {
                return BadRequest(new { message = "incorrect Id" });
            }
            var allLike=await blogPostLikeRepository.GetAllLikes(guidPostId);
            return Ok(allLike);
        }


    }
}
