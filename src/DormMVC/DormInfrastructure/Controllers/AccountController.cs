using DormDomain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DormInfrastructure.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using DocumentFormat.OpenXml.Spreadsheet;


public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly IEmailSender _emailSender;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string ManagerAssistant = "Manager Assistant";
    }


    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email, // keep UserName as email (for login)
                Email = model.Email,
                Name = model.Name // assign real name
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            

            if (result.Succeeded)
            {
                //await _signInManager.SignInAsync(user, isPersistent: false);
                //await _userManager.AddToRoleAsync(user, "Admin");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Підтвердіть вашу пошту",
                    $"Щоб завершити створення акаунту, перейдіть за <a href='{confirmationLink}'>посиланням.</a>.");

                return RedirectToAction("Login", "Account");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("", "Невдала спроба входу.");
        return View(model);
    }

    //public async Task<IActionResult> Logout()
    //{
    //    await _signInManager.SignOutAsync();
    //    return RedirectToAction("Login");
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home"); 
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var roles = await _userManager.GetRolesAsync(user); 

        var model = new UserProfileViewModel
        {
            Email = user.Email,
            UserName = user.UserName,
            Name = user.Name, 
            Roles = roles.ToList()
        };


        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
            return RedirectToAction("Index", "Home");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return View("ConfirmEmail");

        return View("Error");
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

        List<string> allowedRoles;

        if (currentUserRoles.Contains(AppRoles.Admin))
        {
            // Admin can assign any role
            allowedRoles = new List<string> { AppRoles.Admin, AppRoles.Manager, AppRoles.ManagerAssistant};
        }
        else if (currentUserRoles.Contains(AppRoles.Manager))
        {
            // Manager can assign only assistant and student roles
            allowedRoles = new List<string> { AppRoles.ManagerAssistant};
        }
        else
        {
            // Shouldn't happen due to [Authorize] but fallback safety
            return Forbid();
        }

        var model = new RoleManagementViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            CurrentRoles = await _userManager.GetRolesAsync(user),
            AvailableRoles = allowedRoles
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
    public async Task<IActionResult> ManageRoles(RoleManagementViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

        List<string> allowedRoles = new();

        if (currentUserRoles.Contains(AppRoles.Admin))
            allowedRoles = new() { AppRoles.Admin, AppRoles.Manager, AppRoles.ManagerAssistant};
        else if (currentUserRoles.Contains(AppRoles.Manager))
            allowedRoles = new() { AppRoles.ManagerAssistant};

        if (!allowedRoles.Contains(model.SelectedRole))
            return Forbid();

        //var user = await _userManager.FindByIdAsync(userId);
        var userRoles = await _userManager.GetRolesAsync(user);

        //var currentUser = await _userManager.GetUserAsync(User);
        //var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

        if (currentUserRoles.Contains("Manager") && userRoles.Contains("Admin"))
        {
            return Forbid(); // Or redirect/message
        }

        //var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        await _userManager.AddToRoleAsync(user, model.SelectedRole);

        return RedirectToAction("UserList"); // Or wherever you want to go
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UserList()
    {
        var users = _userManager.Users.ToList();
        var currentUserId = _userManager.GetUserId(User);


        var model = new UserListViewModel
        {
            Users = new List<UserListItemViewModel>()
        };

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            model.Users.Add(new UserListItemViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            });
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user doesn't exist or is not confirmed
            return RedirectToAction("Login");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, protocol: Request.Scheme);

        await _emailSender.SendEmailAsync(
            model.Email,
            "Зміна паролю",
            $"Ви можете змінити пароль, перейшовши за цим <a href='{callbackUrl}'>посиланням</a>.");

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null) return RedirectToAction("Index", "Home");

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return RedirectToAction("Login");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            TempData["ToastMessage"] = "Лист для зміни паролю надісланий!";

            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Message"] = "Користувача не знайдено.";
            return RedirectToAction("UsersList");
        }

        // Prevent deleting yourself (optional safety)
        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Message"] = "Ви не можете видалити себе.";
            return RedirectToAction("UsersList");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["Message"] = "Користувача успішно видалено.";
        }
        else
        {
            TempData["Message"] = "Помилка видалення користувача.";
        }

        return RedirectToAction("UsersList");
    }

}
