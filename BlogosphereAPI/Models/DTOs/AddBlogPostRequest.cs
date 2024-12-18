using BlogosphereAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogosphereAPI.Models.DTOs
{
    public class AddBlogPostRequest
    {
        public Guid Id { get; set; }
        public string Heading { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string PageTitle { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool Visible { get; set; }
        //Collect Tags
        public string[] SelectedTags { get; set; } = Array.Empty<string>();
    }
}
