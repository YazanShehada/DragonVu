using DragonVu.Data;
using DragonVu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Data;

namespace DragonVu.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class ManagementController : Controller
    {
        public ManagementController(AppDbContext context,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }
        readonly AppDbContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly IWebHostEnvironment _env;


        [Authorize(Roles = "Admin,Editor")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {

            ViewBag.NewQuestions = _context.questions
                .Count(q => q.Status == QuestionStatus.PendingAdd);

            ViewBag.DeleteRequests = _context.questions
                .Count(q => q.Status == QuestionStatus.PendingDelete);

            ViewBag.ApprovedQuestions = _context.questions
                .Count(q => q.Status == QuestionStatus.Approved);

            ViewBag.SubjectsCount = _context.Set<Subject>().Count();

            //ViewBag.UsersCount = _userManager.Users.Count();
            var users = _userManager.Users.ToList();
            int reault = 0; // Fix: Initialize the variable before use
            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                if (!roles.Contains("User"))
                {
                    reault += 1;
                    ViewBag.UsersCount = reault.ToString();
                }
            }
            return View();
        }
        public async Task<IActionResult> AddRoles()
        {
            var users = _userManager.Users.ToList();

            var data = new List<dynamic>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("User"))
                {
                    data.Add(new
                    {
                        Username = user.UserName,
                        Role = roles.FirstOrDefault() ?? "لا يوجد"
                    });
                }

            }

            ViewBag.UsersWithRoles = data;

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoles(string? dummy = null)
        {
            // جلب القيم من الفورم
            var username = Request.Form["Username"].ToString(); // استخدم اسم المستخدم بدل Id
            var role = Request.Form["Role"].ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                TempData["ErrorMessage"] = "اسم المستخدم أو الدور فارغ!";
                return View("AddRoles");
            }

            // البحث عن المستخدم بالاسم
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود!";
                return View("AddRoles");
            }

            // التأكد من وجود الدور
            if (!await _roleManager.RoleExistsAsync(role))
            {
                TempData["ErrorMessage"] = "الدور غير موجود!";
                return View("AddRoles");
            }

            // إزالة كل الأدوار القديمة
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // إضافة الدور الجديد
            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"تم تعيين الدور '{role}' للمستخدم '{username}' بنجاح ✅";
            }
            else
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تعيين الدور!";
            }

            return RedirectToAction("AddRoles");
        }



        [HttpGet]
        public IActionResult AddSubjects()
        {
            ViewBag.Subjects = _context.Set<Subject>().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSubjects(Subject subject)
        {
            if (ModelState.IsValid)
            {
                if (subject.Id > 0)
                {
                    subject.CreatedAt = DateTime.Now;
                    _context.Update(subject);
                }
                else
                {
                    subject.CreatedAt = DateTime.Now;
                    _context.Add(subject);
                }

                _context.SaveChanges();
                string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images/Questions/subjects",
                 subject.Id.ToString()
                  );

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return RedirectToAction("AddSubjects");

            }
            else
            {
                return View(subject);
            }

        }

        public IActionResult DeleteSubject()
        {
            ViewBag.Subjects = _context.Set<Subject>().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSubject(int id)
        {
            var subject = _context.Set<Subject>().Find(id);
            if (subject != null)
            {
                _context.Set<Subject>().Remove(subject);
                _context.SaveChanges();
            }

            return RedirectToAction("AddSubjects");
        }
        public IActionResult AproveQuestions()
        {
            var pendingQuestions = _context.questions
            .Where(q => q.Status == QuestionStatus.PendingAdd)
            .ToList();

            return View(pendingQuestions);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AproveQuestions(int QuestionId, string actionType)
        {
            var qus = _context.questions.Find(QuestionId);

            if (qus != null)
            {
                if (actionType == "approve")
                {
                    qus.Status = QuestionStatus.Approved;
                    _context.Update(qus);
                    await _context.SaveChangesAsync();
                }
                else if (actionType == "reject")
                {
                    qus.Status = QuestionStatus.Rejected;
                    _context.Update(qus);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("AproveQuestions");
        }

        public IActionResult DeleteRequests()
        {
            var pendingDeleteQuestions = _context.questions
            .Where(q => q.Status == QuestionStatus.PendingDelete)
            .ToList();
            return View(pendingDeleteQuestions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequests(int QuestionId, string actionType)
        {
            var qus = _context.questions.Find(QuestionId);
            if (qus != null)
            {
                if (actionType == "approve")
                {
                    _context.questions.Remove(qus);
                    string pathImg = Path.Combine(_env.WebRootPath, qus.ImageUrl);
                    System.IO.File.Delete(pathImg);
                    await _context.SaveChangesAsync();
                }
                else if (actionType == "reject")
                {
                    qus.Status = QuestionStatus.Approved;
                    _context.Update(qus);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("DeleteRequests");
        }
        public IActionResult ShowQuestions(int? SubjectId)
        {
            var subjects = _context.subjects.ToList();

            ViewBag.Subjects = new SelectList(
                subjects,
                "Id",
                "Name",
                SubjectId // حتى يبقى المختار محدد
            );

            var questions = _context.questions
                .Where(q => q.Status == QuestionStatus.Approved);

            // إذا تم اختيار مادة
            if (SubjectId.HasValue)
            {
                questions = questions.Where(q => q.SubjectId == SubjectId.Value);
            }

            return View(questions.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ShowQuestions(
        int Id,
        int SubjectId,
        int CorrectAnswer,
        string? Text,
        string? AnswerA,
        string? AnswerB,
        string? AnswerC,
        string? AnswerD,
        string? Hint,
        IFormFile? QuestionImage)
        {
            var userId = _userManager.GetUserId(User);

            var question = _context.questions
                .FirstOrDefault(q => q.Id == Id && q.CreatedById == userId);

            if (question == null)
                return NotFound();

            if (SubjectId != 0)
                question.SubjectId = SubjectId;

            question.AnswerA = AnswerA;
            question.AnswerB = AnswerB;
            question.AnswerC = AnswerC;
            question.AnswerD = AnswerD;
            question.CorrectAnswer = CorrectAnswer;
            question.Hint = Hint;
            question.Text = Text;

            // ===== تحديث الصورة إذا تم رفع صورة جديدة =====
            if (QuestionImage != null && QuestionImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(QuestionImage.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Invalid image format");
                    return RedirectToAction("EditeQuestion");
                }

                string folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/Questions/subjects",
                    question.SubjectId.ToString()
                );

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // اسم جديد مع WebP
                string fileName = Guid.NewGuid().ToString() + ".webp";
                string filePath = Path.Combine(folderPath, fileName);
                // ✅ تحويل + ضغط + resize
                using (var image = await Image.LoadAsync(QuestionImage.OpenReadStream()))
                {
                    // Resize إذا أكبر من 1000px
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(1000, 1000)
                    }));

                    var encoder = new WebpEncoder
                    {
                        Quality = 75 // ضغط مناسب
                    };

                    await image.SaveAsync(filePath, encoder);
                }

                // حذف الصورة القديمة (احترافي)
                if (!string.IsNullOrEmpty(question.ImageUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", question.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                question.ImageUrl = $"images/Questions/subjects/{question.SubjectId}/{fileName}";
            }

            // ===== لو كان Editor نرجع الحالة للمراجعة =====
            if (User.IsInRole("Editor"))
                question.Status = QuestionStatus.PendingAdd;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تعديل السؤال بنجاح ✅";
            TempData["UpdatedQuestionId"] = Id;

            return RedirectToAction("ShowQuestions");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult StatisticsPage()
        {
            List<Result> results = _context.results
                .Include(r => r.User)
                .Include(r => r.Subject)
                .ToList();
            var statistics = new StatisticsVM
            {
                results = results,
                allUsers = _userManager.Users.ToList(),
                UserCount = _userManager.Users.Count(),
                latestUsers = _userManager.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToList()
            };
            return View(statistics);
        }
        public IActionResult ManageUsers()
        {
            List<Result> results = _context.results
              .Include(r => r.User)
              .Include(r => r.Subject)
              .ToList();
            var statistics = new StatisticsVM
            {

                allUsers = _userManager.Users.ToList(),

            };

            return View(statistics);
        }
        [HttpPost]
        public IActionResult ManageUsers(string id, string action)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return RedirectToAction("StatisticsPage");

            switch (action)
            {
                case "Activate":
                    user.IsActive = true;
                    break;

                case "Deactivate":
                    user.IsActive = false;
                    break;

                case "Delete":
                    _context.Users.Remove(user);
                    break;
            }

            _context.SaveChanges();

            return RedirectToAction("StatisticsPage");
        }
    }
}


