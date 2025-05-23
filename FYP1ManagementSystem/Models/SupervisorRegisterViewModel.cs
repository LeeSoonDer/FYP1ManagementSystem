using System.ComponentModel.DataAnnotations;

namespace FYP1ManagementSystem.Models
{
    public class SupervisorRegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Faculty { get; set; }

        [Required]
        [Display(Name = "Academic Program")]
        public string AcademicProgram { get; set; } // Data Engineering / Software Engineering

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
