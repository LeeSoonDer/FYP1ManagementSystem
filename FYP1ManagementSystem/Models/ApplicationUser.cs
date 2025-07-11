﻿using Microsoft.AspNetCore.Identity;

namespace FYP1ManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Faculty { get; set; }
        public string? AcademicProgram { get; set; } // Data Engineering / Software Engineering
        public string? Domain { get; set; } // Research / Development
        public bool IsCommittee { get; set; } = false; 
        public string? Year { get; set; } // Year 3 / 4
        public string? Semester { get; set; } // Semester 1 / 2
        public string? AcademicSession { get; set; } // 2024/2025 
        public string? SupervisorId { get; set; }
        public bool IsSupervisorApproved { get; set; } = false; 
        public bool IsStudentAgreed { get; set; } = false;
        public bool IsSupervisorAgreed { get; set; } = false;

    }
}
