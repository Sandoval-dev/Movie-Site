using System.ComponentModel.DataAnnotations;

namespace MovieSite.Models;

public class RoleCreateModel
{
    [Required(ErrorMessage = "Role Name is required.")]
    [StringLength(30, ErrorMessage = "Role Name cannot be longer than 30 characters.")]
    [Display(Name = "Role Name")]
    public string RoleName { get; set; } = null!;

}