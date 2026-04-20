using DogtoraliaMVC.Data;
using DogtoraliaMVC.Models;
using DogtoraliaMVC.Services;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DogtoraliaMVC.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly DogtoraliaDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        DogtoraliaDbContext context,
        IEmailService emailService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await _context.PetOwners.AnyAsync(o => o.Email == vm.Email))
        {
            ModelState.AddModelError(nameof(vm.Email), "Ya existe una cuenta con este correo electrónico.");
            return View(vm);
        }

        var user = new IdentityUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, "User");

        _context.PetOwners.Add(new PetOwner
        {
            Name = vm.Name,
            Email = vm.Email,
            Phone = vm.Phone,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendWelcomeEmailAsync(vm.Email, vm.Name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Welcome email could not be sent to {Email}", vm.Email);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> MyProfile()
    {
        var userId = _userManager.GetUserId(User);
        var owner = await _context.PetOwners.FirstOrDefaultAsync(o => o.UserId == userId);
        if (owner == null) return NotFound();
        return RedirectToAction("Details", "PetOwners", new { id = owner.Id });
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["SuccessMessage"] = "Contraseña cambiada exitosamente.";

        if (User.IsInRole("User"))
        {
            var owner = await _context.PetOwners.FirstOrDefaultAsync(o => o.UserId == user.Id);
            if (owner != null)
                return RedirectToAction("Details", "PetOwners", new { id = owner.Id });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
