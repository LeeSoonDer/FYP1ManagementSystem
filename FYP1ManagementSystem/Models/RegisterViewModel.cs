using System.ComponentModel.DataAnnotations;

namespace FYP1ManagementSystem.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Faculty")]
        public string Faculty { get; set; }

        [Required]
        [Display(Name = "Year")]
        public string Year { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public string Semester { get; set; }

        [Required]
        [Display(Name = "Academic Session")]
        [RegularExpression(@"^\d{4}/\d{4}$", ErrorMessage = "Format should be YYYY/YYYY")]
        public string AcademicSession { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
