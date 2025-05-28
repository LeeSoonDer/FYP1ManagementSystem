using System.Collections.Generic;

namespace FYP1ManagementSystem.Models
{
    public class CommitteeDashboardViewModel
    {
        public List<Proposal> Proposals { get; set; } = new();
        public List<ApplicationUser> Evaluators { get; set; } = new();
        public List<ApplicationUser> Supervisors { get; set; } = new();

        public List<ApplicationUser> StudentsWithSupervisor { get; set; } = new();

    }
}
