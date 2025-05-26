using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieSite.Data;
using MovieSite.Models;

namespace MovieSite.Controllers;

[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly DataContext _context;

    public CategoryController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index()
    {
        var categories = _context.Categories.Select(i => new CategoryGetModel
        {
            Id = i.Id,
            Name = i.Name,
            MovieCount = i.Movies.Count()
        }).ToList();
        return View(categories);
    }


    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(CategoryCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Name = model.Name,
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var category = _context.Categories.FirstOrDefault(i => i.Id == id);
        if (category != null)
        {
            return View(category);
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
        var category = _context.Categories.FirstOrDefault(i => i.Id == id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["Message"] = $"{category.Name} Category deleted successfully.";
            return RedirectToAction("Index");
        }
        return RedirectToAction("Index");
    }


    public ActionResult Edit(int id)
    {
        var category = _context.Categories.Select(i => new CategoryEditModel
        {
            Id = i.Id,
            Name = i.Name
        }).FirstOrDefault(i => i.Id == id);
        return View(category);
    }


    [HttpPost]
    public ActionResult Edit(int id, CategoryEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            var category = _context.Categories.FirstOrDefault(i => i.Id == id);
            if (category != null)
            {
                category.Name = model.Name;
                _context.SaveChanges();
                TempData["Message"] = $"{category.Name} Category updated successfully.";
                return RedirectToAction("Index");
            }
            return View(model);
        }
        return View(model);
    }

}