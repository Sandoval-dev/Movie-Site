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


    public async Task<IActionResult> Index(string sort = "date_desc", int page = 1, string q = null!, string category = null!)
    {
        var movies = _context.Movies
            .Include(m => m.Category) // Kategoriye göre filtreleme yapacağımız için Include şart
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            movies = movies.Where(m => m.Title.Contains(q));

        if (!string.IsNullOrWhiteSpace(category))
            movies = movies.Where(m => m.Category.Name == category);

        movies = sort switch
        {
            "date_asc" => movies.OrderBy(m => m.ReleaseDate),
            "rating_desc" => movies.OrderByDescending(m => m.Rating),
            "rating_asc" => movies.OrderBy(m => m.Rating),
            "title_asc" => movies.OrderBy(m => m.Title),
            "title_desc" => movies.OrderByDescending(m => m.Title),
            _ => movies.OrderByDescending(m => m.ReleaseDate)
        };

        // Sayfalama
        int pageSize = 12;
        int totalMovies = await movies.CountAsync();
        var pagedMovies = await movies.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling(totalMovies / (double)pageSize);
        ViewData["Sort"] = sort;
        ViewData["q"] = q;
        ViewData["category"] = category;

        return View(pagedMovies);
    }


}