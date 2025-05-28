using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FYP1ManagementSystem.Models;
using FYP1ManagementSystem.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace FYP1ManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            var student = await _userManager.GetUserAsync(User);
            var proposals = await _context.Proposals
                .Where(p => p.StudentId == student.Id)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();

            return View(student);
        }


        public async Task<IActionResult> ChooseSupervisor()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            var supervisors = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Supervisor"))
                {
                    supervisors.Add(user);
                }
            }

            return View(supervisors);
        }


        [HttpPost]
        public async Task<IActionResult> SelectSupervisor(string supervisorId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && await _userManager.IsInRoleAsync(user, "Student"))
            {
                user.SupervisorId = supervisorId;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("StudentDashboard");
        }


        [HttpGet]
        public IActionResult SubmitProposal()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitProposal(ProposalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = await _userManager.GetUserAsync(User);
                string? pdfPath = null;

                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    // ✅ 强化文件类型验证
                    var extension = Path.GetExtension(model.UploadedFile.FileName);
                    if (string.IsNullOrEmpty(extension) || extension.ToLower() != ".pdf")
                    {
                        ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                        return View(model);
                    }

                    // （可选）进一步检查 MIME 类型
                    if (model.UploadedFile.ContentType != "application/pdf")
                    {
                        ModelState.AddModelError("UploadedFile", "Uploaded file must be a valid PDF.");
                        return View(model);
                    }

                    // ✅ 保存 PDF 文件
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploads);

                    var fileName = $"{Guid.NewGuid()}_{model.UploadedFile.FileName}";
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }

                    pdfPath = "/uploads/" + fileName;
                }

                // ✅ 提案版本控制
                var lastVersion = await _context.Proposals
                    .Where(p => p.StudentId == student.Id)
                    .OrderByDescending(p => p.Version)
                    .Select(p => p.Version)
                    .FirstOrDefaultAsync();

                var proposal = new Proposal
                {
                    Title = model.Title,
                    ProjectType = model.ProjectType,
                    PdfPath = pdfPath,
                    StudentId = student.Id,
                    SubmittedAt = DateTime.Now,
                    Version = lastVersion + 1
                };

                _context.Proposals.Add(proposal);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Proposal submitted successfully.";
                return RedirectToAction("StudentDashboard");
            }

            return View(model);
        }



        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> SupervisorDashboard(string semester, string session)
        {
            var supervisor = await _userManager.GetUserAsync(User);
            if (supervisor == null || !(await _userManager.IsInRoleAsync(supervisor, "Supervisor")))
                return Unauthorized();

            var myStudents = await _userManager.Users
                .Where(u => u.SupervisorId == supervisor.Id &&
                    (string.IsNullOrEmpty(semester) || u.Semester == semester) &&
                    (string.IsNullOrEmpty(session) || u.AcademicSession == session))
                .ToListAsync();

            var proposals = await _context.Proposals
                .Where(p => myStudents.Select(s => s.Id).Contains(p.StudentId))
                .ToListAsync();

            var viewModel = new SupervisorDashboardViewModel
            {
                Students = myStudents,
                Proposals = proposals
            };

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> ViewProposal(int id)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null) return NotFound();

            return View(proposal);
        }

        [Authorize]
        public async Task<IActionResult> ProposalHistory(string studentId = null)
        {
            var user = await _userManager.GetUserAsync(User);

            // 如果访问的是其他人的数据
            if (!string.IsNullOrEmpty(studentId) && studentId != user.Id)
            {
                // 如果不是 Committee，就禁止访问
                if (!user.IsCommittee)
                {
                    return Forbid(); // 或 RedirectToAction("AccessDenied")
                }

                var targetStudent = await _userManager.FindByIdAsync(studentId);
                ViewBag.TargetStudentName = targetStudent?.FullName ?? "Unknown Student";
            }

            var targetStudentId = studentId ?? user.Id;

            var proposals = await _context.Proposals
                .Where(p => p.StudentId == targetStudentId)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();

            return View(proposals);
        }




        [HttpPost]
        public async Task<IActionResult> AddSupervisorComment(int id, string comment)
        {
            var proposal = await _context.Proposals.FindAsync(id);
            if (proposal == null) return NotFound();

            if (!string.IsNullOrEmpty(proposal.Evaluation1Status) || !string.IsNullOrEmpty(proposal.Evaluation2Status))
            {
                TempData["Error"] = "You cannot modify comment after evaluators have started.";
                return RedirectToAction("ViewProposal", new { id });
            }

            proposal.SupervisorComments = comment;
            await _context.SaveChangesAsync();

            return RedirectToAction("SupervisorDashboard");
        }

        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CommitteeDashboard()
        {
            var students = await _userManager.Users
                .Where(u => u.SupervisorId != null)
                .ToListAsync();

            var proposals = await _context.Proposals
                .Include(p => p.Student)
                .ToListAsync();

            var evaluators = await _userManager.Users
                .Where(u => u.AcademicProgram != null || u.IsCommittee)
                .ToListAsync();

            var model = new CommitteeDashboardViewModel
            {
                StudentsWithSupervisor = students,
                Proposals = proposals,
                Evaluators = evaluators
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEvaluators(int proposalId, string evaluator1Id, string evaluator2Id)
        {
            if (evaluator1Id == evaluator2Id)
            {
                ModelState.AddModelError("", "Evaluator 1 and 2 cannot be the same.");
                return RedirectToAction("CommitteeDashboard");
            }

            var proposal = await _context.Proposals.FindAsync(proposalId);
            if (proposal != null)
            {
                var student = await _userManager.FindByIdAsync(proposal.StudentId);

                if (evaluator1Id != student?.SupervisorId && evaluator2Id != student?.SupervisorId)
                {
                    proposal.Evaluator1Id = evaluator1Id;
                    proposal.Evaluator2Id = evaluator2Id;

                    proposal.Status = "Rejected";

                    _context.Proposals.Update(proposal);
                    await _context.SaveChangesAsync();

                    TempData["AssignSuccess"] = "✅ Evaluators assigned successfully!";

                }
                else
                {
                    ModelState.AddModelError("", "Cannot assign the supervisor of this student as evaluator.");
                }
            }

            return RedirectToAction("CommitteeDashboard");
        }


        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> EvaluatorDashboard()
        {
            var evaluator = await _userManager.GetUserAsync(User);
            if (evaluator == null || !(await _userManager.IsInRoleAsync(evaluator, "Supervisor"))) 
            {
                return Unauthorized();
            }

            var proposals = await _context.Proposals
                .Include(p => p.Student)
                .Where(p => p.Evaluator1Id == evaluator.Id || p.Evaluator2Id == evaluator.Id)
                .ToListAsync();

            return View(proposals);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEvaluation(int proposalId)
        {
            var proposal = await _context.Proposals.FindAsync(proposalId);
            var evaluator = await _userManager.GetUserAsync(User);

            if (proposal == null || evaluator == null) return NotFound();

            var statusKey = $"Status_{proposalId}";
            var commentKey = $"Comment_{proposalId}";

            var status = Request.Form[statusKey];
            var comment = Request.Form[commentKey];

            // ✅ 区分是评审1还是评审2
            if (proposal.Evaluator1Id == evaluator.Id)
            {
                proposal.Evaluation1Status = status;
                proposal.Evaluator1Comment = comment;
            }
            else if (proposal.Evaluator2Id == evaluator.Id)
            {
                proposal.Evaluation2Status = status;
                proposal.Evaluator2Comment = comment;
            }

            if (!string.IsNullOrEmpty(proposal.Evaluation1Status) && !string.IsNullOrEmpty(proposal.Evaluation2Status))
            {
                var s1 = proposal.Evaluation1Status;
                var s2 = proposal.Evaluation2Status;

                if (s1 == "Approved" && s2 == "Approved")
                {
                    proposal.Status = "Approved";
                }
                else if (s1 == "Approved with Condition" || s2 == "Approved with Condition")
                {
                    proposal.Status = "Approved with Condition";
                }
                else
                {
                    proposal.Status = "Rejected";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("EvaluatorDashboard");
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet]
        public async Task<IActionResult> EditEvaluation(int id)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
                return NotFound();

            var evaluator = await _userManager.GetUserAsync(User);
            if (evaluator == null)
                return Unauthorized();

            // 确保当前用户是分配的评审人
            bool isEvaluator1 = proposal.Evaluator1Id == evaluator.Id;
            bool isEvaluator2 = proposal.Evaluator2Id == evaluator.Id;

            if (!isEvaluator1 && !isEvaluator2)
                return Forbid();

            // 用 ViewModel 传递必要信息
            var viewModel = new EvaluationEditViewModel
            {
                ProposalId = proposal.ProposalId,
                Title = proposal.Title,
                StudentName = proposal.Student?.FullName,
                CurrentStatus = isEvaluator1 ? proposal.Evaluation1Status : proposal.Evaluation2Status,
                CurrentComment = isEvaluator1 ? proposal.Evaluator1Comment : proposal.Evaluator2Comment
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvaluation(EvaluationEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var proposal = await _context.Proposals.FindAsync(model.ProposalId);
            var evaluator = await _userManager.GetUserAsync(User);

            if (proposal == null || evaluator == null)
                return NotFound();

            bool isEvaluator1 = proposal.Evaluator1Id == evaluator.Id;
            bool isEvaluator2 = proposal.Evaluator2Id == evaluator.Id;

            if (!isEvaluator1 && !isEvaluator2)
                return Forbid();

            if (isEvaluator1)
            {
                proposal.Evaluation1Status = model.CurrentStatus;
                proposal.Evaluator1Comment = model.CurrentComment;
            }
            else if (isEvaluator2)
            {
                proposal.Evaluation2Status = model.CurrentStatus;
                proposal.Evaluator2Comment = model.CurrentComment;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Evaluation updated successfully.";
            return RedirectToAction("EvaluatorDashboard");
        }



        [HttpPost]
        public async Task<IActionResult> AssignDomain(string supervisorId, string domain)
        {
            var user = await _userManager.FindByIdAsync(supervisorId);

            if (user != null && _userManager.IsInRoleAsync(user, "Supervisor").Result)
            {
                user.Domain = domain;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("CommitteeDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSupervisor(string studentId)
        {
            var student = await _userManager.FindByIdAsync(studentId);

            if (student != null && _userManager.IsInRoleAsync(student, "Student").Result)
            {
                student.IsSupervisorApproved = true;
                await _userManager.UpdateAsync(student);
            }

            return RedirectToAction("CommitteeDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgreeWithSupervisor()
        {
            var student = await _userManager.GetUserAsync(User);
            if (student != null)
            {
                student.IsStudentAgreed = true;
                await _userManager.UpdateAsync(student);
            }

            return RedirectToAction("StudentDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgreeWithStudent(string studentId)
        {
            var supervisor = await _userManager.GetUserAsync(User);
            var student = await _userManager.FindByIdAsync(studentId);

            if (student != null && student.SupervisorId == supervisor.Id)
            {
                student.IsSupervisorAgreed = true;
                await _userManager.UpdateAsync(student);
            }

            return RedirectToAction("SupervisorDashboard");
        }

        [HttpGet]
        public async Task<IActionResult> ViewProposalVersion(int id)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null) return NotFound();

            return View(proposal);
        }

        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> ManageStudents(string program = null)
        {
            var committee = await _userManager.GetUserAsync(User);
            if (committee == null || !committee.IsCommittee)
                return Unauthorized();

            // 拉出所有用户（或优化为只取有 SupervisorId 的）
            var allUsers = await _userManager.Users.ToListAsync();

            // 本地过滤角色
            var students = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Student"))
                    students.Add(user);
            }

            if (!string.IsNullOrEmpty(program))
            {
                students = students.Where(s => s.AcademicProgram == program).ToList();
            }

            // dropdown program options
            var programs = allUsers
                .Where(u => !string.IsNullOrEmpty(u.AcademicProgram))
                .Select(u => u.AcademicProgram)
                .Distinct()
                .ToList();

            ViewBag.Programs = programs;
            ViewBag.SelectedProgram = program;

            return View(students);
        }



    }
}
