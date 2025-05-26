
using System.ComponentModel.DataAnnotations;

namespace MovieSite.Models;

public class MovieModel
{
    [Required(ErrorMessage = "Movie Name is required")]
    [StringLength(100, ErrorMessage = "Movie Name cannot be longer than 100 characters")]
    [Display(Name = "Movie Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Movie Description is required")]
    [StringLength(500, ErrorMessage = "Movie Description cannot be longer than 500 characters")]
    [Display(Name = "Movie Description")]
    public string Description { get; set; } = null!;

    public IFormFile? Image { get; set; }

    [Required(ErrorMessage = "Release Date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Release Date")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Rating is required")]
    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    [Display(Name = "Rating")]
    public float Rating { get; set; }

}