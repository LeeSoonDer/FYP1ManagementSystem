using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FYP1ManagementSystem.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace FYP1ManagementSystem.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult CreateSupervisor()
        {
            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupervisor(SupervisorRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Faculty = model.Faculty,
                    AcademicProgram = model.AcademicProgram
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Supervisor");
                    TempData["Success"] = "Supervisor account created successfully!";
                    return RedirectToAction("ManageSupervisors");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult ManageSupervisors()
        {
            var supervisors = _userManager.Users
                .Where(u => u.AcademicProgram != null) // 仅导师
                .ToList();

            return View(supervisors);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCommittee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsCommittee = !user.IsCommittee;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("ManageSupervisors");
        }
    }
}
