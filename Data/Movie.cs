namespace MovieSite.Data;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public int CategoryId { get; set; }
    public float Rating { get; set; }
    public Category Category { get; set; } = null!;
}