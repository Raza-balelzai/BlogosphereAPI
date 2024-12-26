namespace BlogosphereAPI.Models.Domain
{
    public class BlogPostComment
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public Guid Userid { get; set; }
        public Guid BlogPostId { get; set; }
        public DateTime CommentTimeDate { get; set; }
    }
}
