using MovieSite.Data;

namespace MovieSite.Models;

public class AdminDashboardModel
{
    public AppUser CurrentUser { get; set; } = null!; // Ensure this is initialized properly in the controller
    public int TotalUsers { get; set; }
    public int TotalMovies { get; set; }
    public int TotalComments { get; set; }
}