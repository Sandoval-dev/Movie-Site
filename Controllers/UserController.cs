using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieSite.Data;
using MovieSite.Models;

namespace MovieSite.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly DataContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UserController(DataContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string role)
    {
        ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", role);
        if (!string.IsNullOrEmpty(role))
        {
            return View(await _userManager.GetUsersInRoleAsync(role));
        }
        return View(_userManager.Users);
    }


    public async Task<ActionResult> Edit(string id)
    {
        var entity = await _userManager.FindByIdAsync(id);
        if (entity == null)
        {

            return RedirectToAction("Index");
        }
        ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
        var model = new UserEditModel()
        {
            FullName = entity.FullName,
            Email = entity.Email!,
            SelectedRoles = await _userManager.GetRolesAsync(entity),
        };
        return View(model);
    }


    // [HttpPost]
    // public async Task<ActionResult> Edit(string id, UserEditModel model)
    // {
    //     var entity = await _userManager.FindByIdAsync(id);
    //     if (entity == null)
    //     {
    //         return RedirectToAction("Index");
    //     }
    //     entity.FullName = model.FullName;
    //     entity.Email = model.Email;
    //     var result = await _userManager.UpdateAsync(entity);
    //     if (result.Succeeded)
    //     {
    //         var roles = await _userManager.GetRolesAsync(entity);
    //         foreach (var role in roles)
    //         {
    //             await _userManager.RemoveFromRoleAsync(entity, role);
    //         }
    //         foreach (var role in model.SelectedRoles!)
    //         {
    //             await _userManager.AddToRoleAsync(entity, role);
    //         }
    //         return RedirectToAction("Index");
    //     }
    //     return View(model);
    // }


    [HttpPost]
    public async Task<ActionResult> Edit(string id, UserEditModel model)
    {
        var entity = await _userManager.FindByIdAsync(id);
        if (entity == null)
        {
            return RedirectToAction("Index");
        }

        // Kullanıcının mevcut rollerini al
        var currentRoles = await _userManager.GetRolesAsync(entity);

        // Mevcut rollerin hepsini kaldır
        foreach (var role in currentRoles)
        {
            await _userManager.RemoveFromRoleAsync(entity, role);
        }

        // Yeni seçilen rolleri ekle
        if (model.SelectedRoles != null)
        {
            foreach (var role in model.SelectedRoles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    await _userManager.AddToRoleAsync(entity, role);
                }
            }
        }

        return RedirectToAction("Index");
    }


}