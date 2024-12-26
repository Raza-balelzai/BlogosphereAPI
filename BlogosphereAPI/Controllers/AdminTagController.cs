using Azure;
using BlogosphereAPI.Models.Domain;
using BlogosphereAPI.Models.DTOs;
using BlogosphereAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                    message = "Tag already exists",
                    tag = existingTag
                });
            }
            // Add tag using the repository
            var addedTag = await tagRepository.AddTagAsync(tag);
            // Check if the tag was added successfully
            if (addedTag != null)
            {
                return Ok(new
                {
                    message="Tag created successfully",
                }); // Return 200 OK with the added tag
            }
            // If the tag was not added, return an error response
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "Unable to create the tag. Please try again later."
            });
        }

        [HttpPut("{id}")] // This route accepts a Name as a URL parameter for identifying the tag to update 
        public async Task<IActionResult> UpdateTag([FromBody] TagDto _tag, [FromRoute] string id)
        {
            if (_tag == null || id == null)
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
            var Guidid = Guid.Parse(id);
            // Call the repository to update the tag in the database
            var updatedTag = await tagRepository.UpdateTagAsync(tag, Guidid);
            // Check if the update was successful (if a tag was found and updated)

            if (updatedTag != null)
            {
                TagDto tagDto = new TagDto
                {
                    Id=updatedTag.Id,
                    Name=updatedTag.Name,
                    DisplayName=updatedTag.DisplayName
                };
                // Return the updated tag as the response with a 200 OK status
                return Ok(new
                {
                    tagDto,
                    Message="Tag successfully updated."
                });
            }
            else
            {
                // Return a 400 Bad Request response with an appropriate message
                return BadRequest("Bad request! Tag already exist or no such tag found to update");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "Tag ID is required." });
            }

            try
            {
                // Try to parse the ID as a GUID
                if (!Guid.TryParse(id, out Guid guidId))
                {
                    return BadRequest(new { message = "Invalid Tag ID format." });
                }

                var foundTag = await tagRepository.GetByIdAsync(guidId);
                

                if (foundTag != null)
                {
                    TagDto tagDto = new TagDto
                    {
                        Id = foundTag.Id,
                        Name = foundTag.Name,
                        DisplayName = foundTag.DisplayName
                    };
                    return Ok(tagDto); // Return the found tag
                }
                else
                {
                    return NotFound(new { message = "Sorry! No such Tag found." });
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new { message = "An error occurred while fetching the tag.", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag([FromRoute] string? routeId, [FromQuery] string? querryId)
        {
            var id = routeId ?? querryId;
            // Check for null or empty ID
            if (id == null || id == string.Empty)
            {
                return BadRequest(new { message = "Tag ID is required." });
            }

            try
            {
                // Attempt to delete the tag
                var deletedTag = await tagRepository.DeleteTagAsync(id);

                if (deletedTag != null)
                {
                    // Return success with the deleted tag details
                    return Ok(new
                    {
                        message = "Tag deleted successfully.",
                        item = deletedTag.DisplayName
                    });
                }
                else
                {
                    // Return not found if the tag doesn't exist
                    return NotFound(new { message = "No tag found with the provided ID." });
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new { message = "An error occurred while deleting the tag.", error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTags()
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
            return Ok(tagDtos);
        }

    }
}
