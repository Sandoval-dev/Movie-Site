using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieSite.Data;

namespace MovieSite.Controllers;


public class CommentController : Controller
{
    public DataContext _context { get; set; }
    public UserManager<AppUser> _userManager { get; set; }

    public CommentController(DataContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var comments = await _context.Comments
            .Include(c => c.User) // Kullanıcı bilgisini almak için
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentGetModel
            {
                Id = c.Id,
                MovieTitle=c.Movie.Title,
                UserName = c.User.UserName, // Veya c.User.FullName gibi istediğin kullanıcı alanı
                Content = c.Content,
                CreatedDate = c.CreatedAt
            })
            .ToListAsync();

        return View(comments);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CommentViewModel commentViewModel)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Details", "Movie", new { id = commentViewModel.MovieId });


        var userIdStr = _userManager.GetUserId(User);
        int userId = int.Parse(userIdStr!);
        var comment = new Comment
        {
            MovieId = commentViewModel.MovieId,
            UserId = userId,
            Content = commentViewModel.Content
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Movie", new { id = commentViewModel.MovieId });
    }


    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var comment = _context.Comments.FirstOrDefault(i => i.Id == id);
        if (comment != null)
        {
            return View(comment);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteConfirm(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var comment = _context.Comments.FirstOrDefault(i => i.Id == id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            TempData["Message"] = $"{comment.Content} deleted successfully.";
            return RedirectToAction("Index");
        }
        return RedirectToAction("Index");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserDelete(int id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound();

        // Yorum sahibi ile giriş yapan kullanıcı aynı mı kontrolü
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null || comment.UserId != int.Parse(userId))
            return Forbid();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        // Silme sonrası yönlendirme - örn. yorumun olduğu filmin detay sayfasına
        return RedirectToAction("Details", "Movie", new { id = comment.MovieId });
    }

}