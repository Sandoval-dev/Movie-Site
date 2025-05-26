using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSite.Data;

namespace MovieSite.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;

    public HomeController(DataContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index(string q, int page = 1, int pageSize = 10, int? category = null)
    {
        var query = _context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(q))
            query = query.Where(m => m.Title.Contains(q));

        if (category.HasValue)
            query = query.Where(m => m.CategoryId == category.Value);

        int totalItems = await query.CountAsync();
        var movies = await query
            .OrderBy(m => m.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate,
                Category=m.Category,
                ImageUrl = m.ImageUrl
            })
            .ToListAsync();

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewData["q"] = q;
        ViewData["category"] = category;

        return View(movies);
    }

}