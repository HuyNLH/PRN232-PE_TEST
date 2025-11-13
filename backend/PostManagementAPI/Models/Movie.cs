namespace PostManagementAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Genre { get; set; }
        public int? Rating { get; set; } // 1-5 or null
        public string? PosterImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
