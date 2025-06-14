using Microsoft.AspNetCore.Identity;

namespace MovieSite.Data;

public class AppUser : IdentityUser<int>
{
    public string FullName { get; set; } = null!;
    public string ProfileImage { get; set; } = null!;
}