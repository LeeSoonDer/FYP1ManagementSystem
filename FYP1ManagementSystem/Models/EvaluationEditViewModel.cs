using System.ComponentModel.DataAnnotations;

namespace FYP1ManagementSystem.Models
{
    public class EvaluationEditViewModel
    {
        public int ProposalId { get; set; }

        public string Title { get; set; }

        public string StudentName { get; set; }

        [Required]
        public string CurrentStatus { get; set; }

        public string CurrentComment { get; set; }
    }
}
