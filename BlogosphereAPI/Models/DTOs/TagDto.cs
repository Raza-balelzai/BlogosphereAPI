using BlogosphereAPI.Models.Domain;

namespace BlogosphereAPI.Models.DTOs
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        
    }
}
