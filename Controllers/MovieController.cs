using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieSite.Data;
using MovieSite.Models;

namespace MovieSite.Controllers;


[Authorize(Roles = "Admin")]
public class MovieController : Controller
{
    private readonly DataContext _context;
    private readonly UserManager<AppUser> _userManager;
    public MovieController(DataContext context, UserManager<AppUser> userManager)
    {
        _userManager = userManager;
        _context = context;
    }


    public async Task<IActionResult> Index(string q, int page = 1, int pageSize = 10, int? category = null)
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

        var query = _context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(q))
            query = query.Where(m => m.Title.ToLower().Contains(q.ToLower()));

        if (category.HasValue)
            query = query.Where(m => m.CategoryId == category.Value);

        int totalItems = await query.CountAsync();

        var movies = await query
            .OrderBy(m => m.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MovieGetModel
            {
                Id = m.Id,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate,
                CategoryName = m.Category.Name,
                ImageUrl = m.ImageUrl
            })
            .ToListAsync();

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewData["q"] = q;
        ViewData["category"] = category;

        return View(movies);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies
             .Include(m => m.Category)
             .Include(m => m.Comments)
             .ThenInclude(c => c.User)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        // Similar movies getirme örneği
        var similarMovies = await _context.Movies
            .Where(m => m.CategoryId == movie.CategoryId && m.Id != movie.Id)
            .OrderByDescending(m => m.Rating)
            .Take(4)
            .ToListAsync();

        ViewData["SimilarMovies"] = similarMovies;

        // Kullanıcının favori listesinde olan filmleri getir
        var userMovieIds = new List<int>();

        if (User.Identity!.IsAuthenticated)
        {
            var userIdStr = _userManager.GetUserId(User);
            if (int.TryParse(userIdStr, out int userId))
            {
                userMovieIds = await _context.MovieLists
                    .Where(ml => ml.UserId == userId)
                    .Select(ml => ml.MovieId)
                    .ToListAsync();
            }
        }

        ViewData["UserMovieIds"] = userMovieIds;

        return View(movie);
    }


    [AllowAnonymous]
    public IActionResult List(string categoryName, string q)
    {
        var movies = _context.Movies.Include(c => c.Category).AsQueryable();
        if (!string.IsNullOrEmpty(categoryName))
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category != null)
            {
                movies = movies.Where(m => m.CategoryId == category.Id);
            }
        }
        if (!string.IsNullOrEmpty(q))
        {
            movies = movies.Where(m => m.Title.ToLower().Contains(q.ToLower()));
            ViewData["q"] = q;
        }
        ViewBag.CategoryName = categoryName;

        return View(movies.ToList());
    }

    public ActionResult Delete(int id)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
        if (movie == null)
        {
            return RedirectToAction("Index");
        }
        return View(movie);
    }

    [HttpPost]
    public ActionResult DeleteConfirm(int id)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            TempData["Message"] = $"{movie.Title} Movie deleted successfully.";
            return RedirectToAction("Index");
        }
        return RedirectToAction("Index");
    }

    public ActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(MovieCreateModel model)
    {
        if (model.Image == null || model.Image.Length == 0)
        {
            ModelState.AddModelError("Image", "Image is required.");
        }
        if (ModelState.IsValid)
        {
            var filename = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", filename);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.Image!.CopyToAsync(stream);
            }
            var movie = new Movie
            {
                Title = model.Name,
                Description = model.Description,
                Rating = model.Rating,
                CategoryId = (int)model.CategoryId,
                ReleaseDate = model.ReleaseDate,
                ImageUrl = filename
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", model.CategoryId);
        return View(model);
    }

    public ActionResult Edit(int id)
    {
        var movie = _context.Movies.Select(i => new MovieEditModel
        {
            Id = i.Id,
            Name = i.Title,
            Description = i.Description,
            ImageName = i.ImageUrl,
            ReleaseDate = i.ReleaseDate,
            Rating = i.Rating,
            CategoryId = i.CategoryId
        }).FirstOrDefault(i => i.Id == id);
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", movie?.CategoryId);
        return View(movie);
    }


    [HttpPost]
    public async Task<ActionResult> Edit(int id, MovieEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            if (model.Image == null || model.Image.Length == 0)
            {
                ModelState.AddModelError("Image", "Image is required.");
            }
            var movie = _context.Movies.FirstOrDefault(i => i.Id == model.Id);
            if (movie != null)
            {
                if (model.Image != null)
                {
                    var fileName = Path.GetRandomFileName() + ".jpg";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.Image!.CopyToAsync(stream);
                    }
                    movie.ImageUrl = fileName;
                }
                movie.Title = model.Name;
                movie.Description = model.Description;
                movie.Rating = model.Rating;
                movie.CategoryId = (int)model.CategoryId;
                movie.ReleaseDate = model.ReleaseDate;
                _context.SaveChanges();
                TempData["Message"] = $"{movie.Title} Movie updated successfully.";
                return RedirectToAction("Index");
            }
        }
        return View(model);
    }

}