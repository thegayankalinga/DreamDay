using System.Security.Claims;
using DreamDay.Data;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    
    // GET (If you do not specify the [HttpGet] it is always get
    public IActionResult Login()
    {
        //This will hold the value if refreshed the page
        var response = new LoginViewModel();
        return View(response);
    }

        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if(!ModelState.IsValid) return View(loginViewModel);

        var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);
        if (user != null)
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
            if (passwordCheck)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(UserRoles.Planner))
                    {
                        var planner = _context.PlannerProfiles.FirstOrDefault(p => p.AppUserId == user.Id);
                        if (planner == null || !planner.IsApproved)
                        {
                            TempData["Error"] = "Planner account not yet approved.";
                            return View(loginViewModel);
                        }
                    }
                    Console.WriteLine("Login successful!");

                    if (roles.Contains(UserRoles.Planner))
                        return RedirectToAction("Index", "Planner", new { area = "Dashboard" });

                    if (roles.Contains(UserRoles.Couple))
                        return RedirectToAction("Index", "Couple", new { area = "Dashboard" });

                    if (roles.Contains(UserRoles.Admin))
                        return RedirectToAction("Index", "Admin", new { area = "Dashboard" });
                }
            }
            TempData["Error"] = "Invalid username or password 1";
            return View(loginViewModel);
        }
        TempData["Error"] = "Invalid username or password 2";
        return View(loginViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (registerViewModel.Role == "Couple")
        {
            if (!registerViewModel.WeddingDate.HasValue)
            {
                ModelState.AddModelError("WeddingDate", "Wedding date is required for couples.");
            }
            if (string.IsNullOrWhiteSpace(registerViewModel.PartnerName))
            {
                ModelState.AddModelError("PartnerName", "Partner name is required for couples.");
            }
        }

        if(!ModelState.IsValid) return View(registerViewModel);
        var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
        if (user != null)
        {
            TempData["Error"] = "Email address already exists, Please Login";
            return View(registerViewModel);
        }

        var newUser = new AppUser()
        {
            UserName = registerViewModel.EmailAddress,
            Email = registerViewModel.EmailAddress,
            FirstName = registerViewModel.FirstName,
            LastName = registerViewModel.LastName,

        };
        var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);
        if (newUserResponse.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, UserRoles.Couple);
            switch (registerViewModel.Role)
            {
                case UserRoles.Couple:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Couple);
                    _context.CoupleProfiles.Add(new CoupleProfile
                    {
                        AppUserId = newUser.Id,
                        WeddingDate = registerViewModel.WeddingDate.Value,
                        PartnerName = registerViewModel.PartnerName
                    });
                    break;

                case UserRoles.Planner:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Planner);
                    _context.PlannerProfiles.Add(new PlannerProfile
                    {
                        AppUserId = newUser.Id,
                        IsApproved = false // Needs admin approval
                    });
                    break;

                case UserRoles.Admin:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                    break;
            }
        }
        return RedirectToAction("Login", "Account");

    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    //View All Planners
    // [Authorize(Roles = UserRoles.Admin)]
    public IActionResult GetPlannerProfiles()
    {
        {
            var planners = _context.PlannerProfiles
                // .Where(p => !p.IsApproved)
                .Include(p => p.AppUser)
                .ToList();
            return View("Planner/ListOfPlanners", planners);
        } 
    }
    
    [HttpPost]
    //[Authorize(Roles = UserRoles.Admin)]
    public IActionResult ApprovePlanner(string plannerId)
    {
        var planner = _context.PlannerProfiles.FirstOrDefault(p => p.AppUserId == plannerId);
        if (planner != null)
        {
            planner.IsApproved = true;
            _context.SaveChanges();
        }
        return RedirectToAction("ApprovePlanners");
    }
    
}