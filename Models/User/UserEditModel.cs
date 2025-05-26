using System.ComponentModel.DataAnnotations;

namespace MovieSite.Models;

public class UserEditModel
{

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public IList<string>? SelectedRoles { get; set; } 


}