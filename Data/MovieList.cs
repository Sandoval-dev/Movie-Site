using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieSite.Data;

public class MovieList
{
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    [ForeignKey("Movie")]
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
}