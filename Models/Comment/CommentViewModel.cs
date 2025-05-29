using System.ComponentModel.DataAnnotations;

public class CommentViewModel
{
    public int MovieId { get; set; }
    [Required]
    public string Content { get; set; } = null!;
}