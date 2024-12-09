using Azure;
using BlogosphereAPI.Models.Domain;
using BlogosphereAPI.Models.DTOs;
using BlogosphereAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogosphereAPI.Controllers
{
    [Route("api/AdminTags/[Action]")]
    [ApiController]
    public class AdminTagController : ControllerBase
    {
        private readonly ITagRepository tagRepository;

        public AdminTagController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagDto _tag)
        {
            if (_tag == null) 
            {
                return BadRequest(new { message = "Tag data is required" });
            }
            // Replace white spaces with underscores in the tag name
            var formattedName = _tag.Name.Replace(" ", "_").ToLower();
            // Map TagDto to Tag
            var tag = new Tag
            {
                Name = formattedName,
                DisplayName = _tag.DisplayName,
            };
            //First check if the same tag exists in DataBase then we will return 404 with the tag.
            var existingTag=await tagRepository.FindByNameAsync(tag.Name);
            if (existingTag != null) 
            {
                return BadRequest(new
                {
                    message = "Sorry, the tag already exists",
                    tag = existingTag
                });
            }
            // Add tag using the repository
            var addedTag = await tagRepository.AddTagAsync(tag);
            // Check if the tag was added successfully
            if (addedTag != null)
            {
                return Ok(addedTag); // Return 200 OK with the added tag
            }
            // If the tag was not added, return an error response
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "Unable to create the tag. Please try again later."
            });
        }

        [HttpPut("{name}")] // This route accepts a Name as a URL parameter for identifying the tag to update 
        public async Task<IActionResult> UpdateTag([FromBody] TagDto _tag, [FromRoute] string name)
        {
            if (_tag == null || name == null)
            {
                return BadRequest(new { message = "Tag data is required" });
            }
            //give proper format to old name (if by chance sent capital in route)
            //and format the new name too
            
            var formattedNewName = _tag.Name.Replace(" ", "_").ToLower();
            // Map the incoming TagDto to a domain model (Tag)
            var tag = new Tag
            {
                Name = formattedNewName, // Assign the Name from TagDto to the domain model
                DisplayName = _tag.DisplayName // Assign the DisplayName from TagDto to the domain model
            };
            // Call the repository to update the tag in the database
            var updatedTag = await tagRepository.UpdateTagAsync(tag, name);
            // Check if the update was successful (if a tag was found and updated)
            if (updatedTag != null)
            {
                // Return the updated tag as the response with a 200 OK status
                return Ok(updatedTag);
            }
            else
            {
                // Return a 400 Bad Request response with an appropriate message
                return BadRequest("Bad request! Tag already exist or no such tag found to update");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTag([FromBody] TagDto _tag)
        {
            //check for null 
            if (_tag == null)
            {
                return BadRequest(new { message = "Tag data is required" });
            }
            // Map the incoming TagDto to a domain model (Tag)
            var tag = new Tag
            {
                Name = _tag.Name, // Assign the Name from TagDto to the domain model
                DisplayName = _tag.DisplayName // Assign the DisplayName from TagDto to the domain model
            };
            // Call the repository to update the tag in the database
            var deletedTag = await tagRepository.DeleteTagAsync(tag.Name);
            // Check if the update was successful (if a tag was found and updated)
            if (deletedTag != null)
            {
                // Return the updated tag as the response with a 200 OK status
                return Ok();
            }
            else
            {
                // Return a 400 Bad Request response with an appropriate message
                return BadRequest("Bad request! No such tag found to Delete.");
            }

        }

        [HttpGet]
        public async Task<IEnumerable<TagDto>> GetAllTags()
        {
           var domainModels= await tagRepository.GetAllTagsAsync();
            // Convert each domain model to a DTO
            var tagDtos = domainModels.Select(model => new TagDto
            {
                Id = model.Id,
                Name = model.Name, 
                DisplayName = model.DisplayName 
            });

            // Return the collection of DTOs
            return tagDtos;
        }

    }
}
