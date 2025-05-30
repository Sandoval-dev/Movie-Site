using System.ComponentModel.DataAnnotations;

namespace MovieSite.Models;

public class AccountEditUserModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [Display(Name = "Full Name")]
    //[RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "User name can only contain letters and numbers.")]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Profile Image")]
    public IFormFile? ProfileImage { get; set; }

    public string? CurrentProfileImage { get; set; }


}