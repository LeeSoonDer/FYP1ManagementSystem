using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FYP1ManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FYP1ManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Student", "Supervisor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            //Admin account
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@fyp.com",
                    FullName = "System Admin",
                    Faculty = "FACULTY OF COMPUTING"
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            //Create 3 students
            var students = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "alice.student",
                    Email = "alice@studentmail.com",
                    FullName = "Alice Tan",
                    Faculty = "FACULTY OF COMPUTING",
                    AcademicSession = "2024/2025",
                    Year = "Year 4",
                    Semester = "Semester 2"
                },
                new ApplicationUser
                {
                    UserName = "brian.student",
                    Email = "brian@studentmail.com",
                    FullName = "Brian Lee",
                    Faculty = "FACULTY OF COMPUTING",
                    AcademicSession = "2024/2025",
                    Year = "Year 3",
                    Semester = "Semester 1"
                },
                new ApplicationUser
                {
                    UserName = "carmen.student",
                    Email = "carmen@studentmail.com",
                    FullName = "Carmen Lim",
                    Faculty = "FACULTY OF COMPUTING",
                    AcademicSession = "2024/2025",
                    Year = "Year 4",
                    Semester = "Semester 1"
                }
            };

            foreach (var student in students)
            {
                if (await userManager.FindByNameAsync(student.UserName) == null)
                {
                    await userManager.CreateAsync(student, "Stud@123");
                    await userManager.AddToRoleAsync(student, "Student");
                }
            }

            // 3️⃣ Create 12 supervisors
            var supervisors = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "dr.chan", Email = "chan@fyp.com", FullName = "Dr. Chan Kok Leong", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Software Engineering" },
                new ApplicationUser { UserName = "dr.teo", Email = "teo@fyp.com", FullName = "Dr. Teo Ai Ling", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Data Engineering" },
                new ApplicationUser { UserName = "dr.lim", Email = "lim@fyp.com", FullName = "Dr. Lim Hong Wei", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Software Engineering" },
                new ApplicationUser { UserName = "dr.tan", Email = "tan@fyp.com", FullName = "Dr. Tan Mei Ling", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Data Engineering" },
                new ApplicationUser { UserName = "dr.ong", Email = "ong@fyp.com", FullName = "Dr. Ong Siew Keong", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Software Engineering" },
                new ApplicationUser { UserName = "dr.yong", Email = "yong@fyp.com", FullName = "Dr. Yong Chee Leong", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Data Engineering" },
                new ApplicationUser { UserName = "dr.lee", Email = "lee@fyp.com", FullName = "Dr. Lee Hui Ching", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Software Engineering" },
                new ApplicationUser { UserName = "dr.khoo", Email = "khoo@fyp.com", FullName = "Dr. Khoo Kok Wai", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Data Engineering" },
                new ApplicationUser { UserName = "dr.wong", Email = "wong@fyp.com", FullName = "Dr. Wong Kah Meng", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Software Engineering" },
                new ApplicationUser { UserName = "dr.goh", Email = "goh@fyp.com", FullName = "Dr. Goh Chee Keong", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Data Engineering" },
                new ApplicationUser { UserName = "dr.lau", Email = "lau@fyp.com", FullName = "Dr. Lau Chin Wei", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Software Engineering" },
                new ApplicationUser { UserName = "dr.hassan", Email = "hassan@fyp.com", FullName = "Dr. Hassan bin Ismail", Faculty = "FACULTY OF COMPUTING", AcademicProgram = "Data Engineering" }
            };

            foreach (var sv in supervisors)
            {
                if (await userManager.FindByNameAsync(sv.UserName) == null)
                {
                    await userManager.CreateAsync(sv, "Sup@123");
                    await userManager.AddToRoleAsync(sv, "Supervisor");
                }
            }
        }
    }
}
