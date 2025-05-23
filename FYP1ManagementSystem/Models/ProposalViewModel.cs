using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FYP1ManagementSystem.Models
{
    public class ProposalViewModel
    {
        [Required]
        [Display(Name = "Project Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Project Type")]
        public string ProjectType { get; set; } // "Research" or "Development"

        [Display(Name = "Upload Signed Proposal Form (PDF)")]
        public IFormFile? UploadedFile { get; set; }
    }
}

