﻿using Microsoft.AspNetCore.Mvc;
using FYP1ManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FYP1ManagementSystem.Data;


namespace FYP1ManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if (string.IsNullOrEmpty(model.Faculty))
                ModelState.AddModelError("Faculty", "Faculty is required.");

            if (string.IsNullOrEmpty(model.Year))
                ModelState.AddModelError("Year", "Year is required.");

            if (string.IsNullOrEmpty(model.Semester))
                ModelState.AddModelError("Semester", "Semester is required.");

            if (string.IsNullOrEmpty(model.AcademicSession) || !Regex.IsMatch(model.AcademicSession, @"^\d{4}/\d{4}$"))
                ModelState.AddModelError("AcademicSession", "Academic Session format must be YYYY/YYYY.");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Faculty = model.Faculty,
                    Year = model.Year,
                    Semester = model.Semester,
                    AcademicSession = model.AcademicSession
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // 添加到默认 Student 角色
                    await _userManager.AddToRoleAsync(user, "Student");

                    TempData["RegisterSuccess"] = "✅ Your account has been created! Please login.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        private bool IsUserEvaluator(string userId)
        {
            return _context.Proposals.Any(p => p.Evaluator1Id == userId || p.Evaluator2Id == userId);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (user != null)
                    {
                        
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                            return RedirectToAction("AdminDashboard", "Admin");

                        if (await _userManager.IsInRoleAsync(user, "Student"))
                            return RedirectToAction("StudentDashboard", "Home");

                        if (await _userManager.IsInRoleAsync(user, "Supervisor"))
                        {
                            if (user.IsCommittee)
                                return RedirectToAction("CommitteeDashboard", "Home");

                            if (IsUserEvaluator(user.Id))
                                return RedirectToAction("EvaluatorDashboard", "Home");

                            return RedirectToAction("SupervisorDashboard", "Home");
                        }


                        
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "User not found.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
