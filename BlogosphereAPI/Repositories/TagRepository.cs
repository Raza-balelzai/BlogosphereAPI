using BlogosphereAPI.Data;
using BlogosphereAPI.Models.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BlogosphereAPI.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BlogosphereDbContext context;

        public TagRepository(BlogosphereDbContext context)
        {
            this.context = context;
        }
        public async Task<Tag> AddTagAsync(Tag tag)
        {
            var res=await FindByNameAsync(tag.Name);
            if (res != null) 
            {
                return null;
            }
            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> DeleteTagAsync(string id)
        {
            var Id=Guid.Parse(id);
            var res= await context.Tags.FirstOrDefaultAsync(x=>x.Id==Id);
            if (res != null)
            { 
                context.Tags.Remove(res);
                await context.SaveChangesAsync();
                return res;
            }
            return null;
        }

        public async Task<Tag> FindByNameAsync(string name)
        {
            return await context.Tags.FirstOrDefaultAsync(t => t.Name== name);
            
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await context.Tags.ToListAsync();
        }

        public async Task<Tag> GetByIdAsync(Guid id)
        {
            var foundTag=await context.Tags.FirstOrDefaultAsync(x=>x.Id == id);
            if (foundTag ==null)
            {
                return null;
            }
            return foundTag;
        }

        public async Task<Tag> UpdateTagAsync(Tag tag, Guid id)
        {
            
            

            // Find the tag using the old name
            var existingTag = await context.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (existingTag != null)
            {
                // Update the properties of the tag
                existingTag.DisplayName = tag.DisplayName;
                existingTag.Name = tag.Name;

                // Save changes to the database
                await context.SaveChangesAsync();
                return existingTag; // Return the updated tag
            }

            return null; // Old name not found
        }

    }
}
