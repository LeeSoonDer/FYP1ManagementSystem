using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP1ManagementSystem.Models
{
    public class Proposal
    {
        [Key]
        public int ProposalId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ProjectType { get; set; } // Research or Development

        public string? PdfPath { get; set; } 

        public string Status { get; set; } = "Pending"; // Accepted, Conditional, Rejected

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public string? EvaluatorComments { get; set; }

        public string? SupervisorComments { get; set; }
        public string? Evaluator1Id { get; set; }
        public string? Evaluator2Id { get; set; }
        public int Version { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }

        [ForeignKey("Evaluator1Id")]
        public ApplicationUser? Evaluator1 { get; set; }

        [ForeignKey("Evaluator2Id")]
        public ApplicationUser? Evaluator2 { get; set; }
        public string? Evaluator1Comment { get; set; }
        public string? Evaluator2Comment { get; set; }
        public string? Evaluation1Status { get; set; } // Accepted / Conditional / Rejected
        public string? Evaluation2Status { get; set; }
    }
}
