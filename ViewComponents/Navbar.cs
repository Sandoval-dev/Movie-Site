using dotnet_store.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using MovieSite.Data;

namespace dotnet_store.ViewComponents;

public class Navbar:ViewComponent
{
    private readonly DataContext _context;
    public Navbar(DataContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        return View(_context.Categories.ToList());
    }
}