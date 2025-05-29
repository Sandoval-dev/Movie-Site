namespace MovieSite.Models;

public class MovieGetModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public string CategoryName { get; set; } = null!;
   
}