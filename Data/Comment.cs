using System.ComponentModel.DataAnnotations;
using MovieSite.Data;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Film ile ilişki
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }

    // Kullanıcı ile ilişki (eğer login sistemi varsa)
    public int UserId { get; set; }
    public AppUser? User { get; set; }

}
