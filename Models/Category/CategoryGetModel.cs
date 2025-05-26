namespace MovieSite.Models;

public class CategoryGetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int MovieCount { get; set; }
}