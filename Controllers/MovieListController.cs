using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSite.Data;

namespace MovieSite.Controllers;

[Authorize]
public class MovieListController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _context;


    public MovieListController(UserManager<AppUser> userManager, DataContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var userIdStr = _userManager.GetUserId(User);

        if (!int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        // Kullanıcının favori filmlerini çekiyoruz (Movie ve Category dahil)
        var movies = _context.MovieLists
            .Where(ml => ml.UserId == userId)
            .Include(ml => ml.Movie)
            .ThenInclude(m => m.Category)
            .Select(ml => ml.Movie)
            .ToList();

        // Kullanıcının favori listesinde olan film Id'lerini alıyoruz (View'da kontrol için)
        var userMovieIds = movies.Select(m => m.Id).ToList();

        ViewData["UserMovieIds"] = userMovieIds;

        return View(movies);
    }

    [HttpPost]
    public async Task<IActionResult> AddToList(int movieId)
    {
        var userIdStr = _userManager.GetUserId(User);
        if (!int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized("Geçersiz kullanıcı.");
        }

        var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);
        if (!movieExists)
        {
            return NotFound("Film bulunamadı.");
        }

        var alreadyExists = await _context.MovieLists
            .AnyAsync(ml => ml.UserId == userId && ml.MovieId == movieId);

        if (!alreadyExists)
        {
            var movieList = new MovieList
            {
                UserId = userId,
                MovieId = movieId
            };

            _context.MovieLists.Add(movieList);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Movie", new { id = movieId });
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Remove(int movieId)
    {
        var userIdStr = _userManager.GetUserId(User);
        if (!int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized("Geçersiz kullanıcı.");
        }

        var movieList = await _context.MovieLists
            .FirstOrDefaultAsync(ml => ml.UserId == userId && ml.MovieId == movieId);

        if (movieList != null)
        {
            _context.MovieLists.Remove(movieList);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

}