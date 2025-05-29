public class CommentGetModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!; // Yorum yapan kişi
    public string Content { get; set; } = null!;  // Yorum içeriği
    public DateTime CreatedDate { get; set; } // Yorum tarihi
     public string MovieTitle { get; set; } = null!;
}
