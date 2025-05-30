namespace MovieSite.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSite.Data;
using MovieSite.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _context;

    public AdminController(UserManager<AppUser> userManager, DataContext context)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var model = new AdminDashboardModel
        {
            CurrentUser = user,
            TotalUsers = await _context.Users.CountAsync(),
            TotalMovies = await _context.Movies.CountAsync(),
            TotalComments = await _context.Comments.CountAsync()
        };

        return View(model);
    }
}