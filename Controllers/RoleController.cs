using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieSite.Data;
using MovieSite.Models;

namespace MovieSite.Controllers;

[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }


    public ActionResult Index()
    {
        return View(_roleManager.Roles);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(RoleCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _roleManager.CreateAsync(new AppRole
            {
                Name = model.RoleName,
            });
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }


    public async Task<ActionResult> Edit(string id)
    {
        var entity = await _roleManager.FindByIdAsync(id);
        if (entity != null)
        {
            return View(new RoleEditModel()
            {
                Id = entity.Id,
                RoleName = entity.Name!
            });
        }
        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<ActionResult> Edit(string id, RoleEditModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = await _roleManager.FindByIdAsync(id);
            if (entity != null)
            {
                entity.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(entity);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
        }
        return View(model);
    }

    public async Task<ActionResult> Delete(string id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role != null)
        {
            ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name!);
            return View(role);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> DeleteConfirm(string id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Message"] = $"{role.Name} Role deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return RedirectToAction("Index");
    }
}