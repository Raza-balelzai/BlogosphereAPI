namespace BlogosphereAPI.Models.DTOs
{
    public class BlogPostResponseDto
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
        public List<TagDto> Tags { get; set; }
        public int LikeCount { get; set; } = 0;
    }
}
