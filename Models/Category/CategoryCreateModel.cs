
using System.ComponentModel.DataAnnotations;

namespace MovieSite.Models;

public class CategoryCreateModel
{
    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = null!;
}