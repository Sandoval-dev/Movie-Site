using System.ComponentModel.DataAnnotations;

namespace MovieSite.Models;

public class AccountResetPasswordModel
{

    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}