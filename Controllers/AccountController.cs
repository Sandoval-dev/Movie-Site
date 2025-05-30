using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieSite.Data;
using MovieSite.Models;
using MovieSite.Services;

namespace MovieSite.Controllers;

public class AccountController : Controller
{
    private UserManager<AppUser> _userManager;

    private SignInManager<AppUser> _signInManager;
    private IEmailService _emailService;
    private RoleManager<AppRole> _roleManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _webHostEnvironment = webHostEnvironment;
    }
    public ActionResult Create()
    {
        return View();
    }

    public ActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                ProfileImage = "default.png" // Varsayılan profil resmi
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Guest"))
                {
                    await _roleManager.CreateAsync(new AppRole { Name = "Guest" });
                }

                // Kullanıcıya Guest rolünü ata
                await _userManager.AddToRoleAsync(user, "Guest");

                return RedirectToAction("Login", "Account");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public ActionResult Login()
    {
        return View();
    }


    [HttpPost]
    public async Task<ActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.IsLockedOut)
                {
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockoutDate - DateTimeOffset.Now;
                    ModelState.AddModelError("", $"Your account is locked out. Try again in {timeLeft?.Minutes + 1} minutes.");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong password.");
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found.");
            }
        }
        return View(model);
    }


    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public ActionResult ForgotPassword()
    {
        return View();
    }


    [HttpPost]
    public async Task<ActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["Message"] = "Email is required.";
            return View();
        }
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["Message"] = "Email not found.";
            return View();
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token });
        var message = $"Please reset your password by clicking here: <a href='http://localhost:5281{url}'>link</a>";

        await _emailService.SendEmailAsync(user.Email!, "Reset Password", message);
        TempData["Message"] = "Password reset link sent to your email.";

        return RedirectToAction("Login");
    }

    public async Task<ActionResult> ResetPassword(string userId, string token)
    {
        if (userId == null || token == null)
        {
            TempData["Message"] = "Invalid password reset link.";
            return RedirectToAction("Login");
        }
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            TempData["Message"] = "User not found.";
            return RedirectToAction("Login");
        }

        var model = new AccountResetPasswordModel
        {
            Token = token,
            Email = user.Email!,
        };
        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> ResetPassword(AccountResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("Login");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["Message"] = "Password reset successfully.";
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    [Authorize]
    public async Task<ActionResult> EditUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            TempData["Message"] = "User not found.";
        }
        return View(new AccountEditUserModel
        {
            FullName = user?.FullName!,
            Email = user?.Email!,
            CurrentProfileImage = user?.ProfileImage

        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AccountEditUserModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.Email = model.Email;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            // Eski fotoğraf varsa sil
            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", user.ProfileImage);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Yeni fotoğrafı kaydet
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(fileStream);
            }

            user.ProfileImage = uniqueFileName;
            model.CurrentProfileImage = uniqueFileName;
        }
        else
        {
            model.CurrentProfileImage = user.ProfileImage;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["Message"] = "Profile updated successfully!";
            return View(model);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
    }


    [Authorize]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> ChangePassword(AccountChangePasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Password changed successfully.";
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordMismatch")
                    {
                        ModelState.AddModelError("OldPassword", "Current password is incorrect.");
                        TempData["Message"] = "Current password is incorrect.";
                    }
                    else
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
        }
        return View(model);
    }

}