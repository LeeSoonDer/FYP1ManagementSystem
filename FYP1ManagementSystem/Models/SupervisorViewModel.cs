using System.Collections.Generic;

namespace FYP1ManagementSystem.Models
{
    public class SupervisorDashboardViewModel
    {
        public List<ApplicationUser> Students { get; set; } = new();
        public List<Proposal> Proposals { get; set; } = new();
    }
}
