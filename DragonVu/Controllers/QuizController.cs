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

namespace DragonVu.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        public QuizController(AppDbContext context,
                               UserManager<ApplicationUser> userManager
                              , IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        readonly AppDbContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public IActionResult Index()
        {

            var subjects = _context.subjects.ToList();

            ViewBag.Subjects = new SelectList(
                subjects,
                "Id",      // value
                "Name" // text

            );
            return View();
        }
        [Authorize(Roles = "Admin,Editor")]
        public IActionResult AddQuestion()
        {
            var subjects = _context.subjects.ToList();

            ViewBag.Subjects = new SelectList(
                subjects,
                "Id",      // value
                "Name"     // text
            );

            return View(new Question());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> AddQuestion(Question question, IFormFile? QuestionImage)
        {
            if (ModelState.IsValid)
            {
                if (QuestionImage != null && QuestionImage.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var extension = Path.GetExtension(QuestionImage.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Invalid image format");
                        return View(question);
                    }

                    // مسار المجلد: uploads/questions/{SubjectId}
                    string folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/Questions/subjects",
                        question.SubjectId.ToString()
                    );

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // اسم عشوائي آمن + تحويل لاحقة WebP
                    string fileName = Guid.NewGuid().ToString() + ".webp";
                    string filePath = Path.Combine(folderPath, fileName);

                    // ✅ التحويل والضغط باستخدام ImageSharp
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

                    // المسار الذي يُخزن في قاعدة البيانات
                    question.ImageUrl = $"images/Questions/subjects/{question.SubjectId}/{fileName}";
                }
                else
                {
                    //ModelState.AddModelError("", "Image is required");
                    //return View(question);
                    question.ImageUrl = "images/default-question.png";

                }

                question.CreatedAt = DateTime.UtcNow;
                question.CreatedById = _userManager.GetUserId(User);

                if (User.IsInRole("Admin"))
                {
                    question.Status = QuestionStatus.Approved;
                    question.ApprovedAt = DateTime.UtcNow;
                    question.ApprovedById = _userManager.GetUserId(User);
                }
                else if (User.IsInRole("Editor"))
                {
                    question.Status = QuestionStatus.PendingAdd;
                }

                _context.questions.Add(question);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = User.IsInRole("Admin")
                    ? "تم إضافة السؤال واعتماده بنجاح ✅"
                    : "تم إرسال السؤال للمراجعة ⏳";

                return RedirectToAction("AddQuestion");
            }

            return View(question);
        }

        public IActionResult EditeQuestion(int? SubjectId)
        {
            var subjects = _context.subjects.ToList();

            ViewBag.Subjects = new SelectList(
                subjects,
                "Id",      // value
                "Name", // text
                SubjectId
            );
            // Fix: Remove incorrect UserManager declaration and get current user id
            var userId = _userManager.GetUserId(User);
            var QuestionsById = _context.questions
                .Where(q => q.CreatedById == userId)
                ;

            if (SubjectId.HasValue)
            {
                QuestionsById = QuestionsById.Where(q => q.SubjectId == SubjectId.Value);
            }

            return View(QuestionsById.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")]

        public async Task<IActionResult> EditQuestion(
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

            return RedirectToAction("EditeQuestion");
        }
        public IActionResult DeleteQuestion()
        {
            return RedirectToAction("EditeQuestion");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")]
        public IActionResult DeleteQuestion(int Id)
        {
            var qus = _context.questions.Find(Id);
            if (User.IsInRole("Admin"))
            {
                if (qus.ImageUrl != "images/default-question.png")
                {
                    string pathImg = Path.Combine(_env.WebRootPath, qus.ImageUrl);
                    if (System.IO.File.Exists(pathImg))
                        System.IO.File.Delete(pathImg);
                }
                _context.Remove(qus);
                _context.SaveChanges();

            }
            else if (User.IsInRole("Editor"))
            {
                qus.Status = QuestionStatus.PendingDelete;
                _context.SaveChanges();
            }
            string returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            // إذا لم يوجد Referer، اذهب للصفحة الافتراضية
            //return RedirectToAction("ShowQuestions");
            return RedirectToAction("EditeQuestion");
        }
        [HttpGet]

        public IActionResult QuickQuiz(int subjectId)
        {
            var ids = _context.questions
                .Where(q => q.SubjectId == subjectId)
                .OrderBy(q => Guid.NewGuid())
                .Take(10)
                .Select(q => q.Id)
                .ToList();

            var firstQuestion = _context.questions
                .FirstOrDefault(q => q.Id == ids[0]);

            var vm = new StepQuizVM
            {
                QuestionIds = ids,
                CurrentIndex = 0,
                Score = 0,
                SubjectId = subjectId,
                CurrentQuestion = firstQuestion
            };

            return View("QuickQuiz", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> QuickQuiz(StepQuizVM model, string? Continue)
        {
            var questionId = model.QuestionIds[model.CurrentIndex];
            var question = _context.questions.FirstOrDefault(q => q.Id == questionId);

            if (question == null)
                return NotFound();

            model.CurrentQuestion = question;

            // إذا ضغط المستخدم على زر متابعة بعد رؤية التلميح
            if (!string.IsNullOrEmpty(Continue))
            {
                model.CurrentIndex++;
                if (model.CurrentIndex >= model.QuestionIds.Count)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var result = new Result
                    {
                        UserId = user.Id,
                        SubjectId = model.SubjectId,
                        Score = model.Score,
                        CreatedAt = DateTime.Now,
                        Name = user.Name,
                        UserName = user.UserName
                    };
                    _context.Add(result);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Result", new { score = model.Score });
                }

                model.CurrentQuestion = _context.questions.First(q => q.Id == model.QuestionIds[model.CurrentIndex]);
                model.ShowHint = false; // إعادة تعيين التلميح
                model.Hint = string.Empty;
                model.SelectedAnswer = null;
                return View(model);
            }

            // تحقق من الإجابة
            if (model.SelectedAnswer == question.CorrectAnswer)
            {
                model.Score += 10;
                model.CurrentIndex++;

                if (model.CurrentIndex >= model.QuestionIds.Count)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var result = new Result
                    {
                        UserId = user.Id,
                        SubjectId = model.SubjectId,
                        Score = model.Score,
                        CreatedAt = DateTime.Now,
                        Name = user.Name,
                        UserName = user.UserName
                    };
                    _context.Add(result);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Result", new { score = model.Score });
                }

                model.CurrentQuestion = _context.questions.First(q => q.Id == model.QuestionIds[model.CurrentIndex]);
                model.ShowHint = false;
                model.Hint = string.Empty;
                model.SelectedAnswer = null;
                return View(model);
            }
            else
            {
                // إجابة خاطئة → أوقف التقدم وأظهر التلميح
                model.ShowHint = true;
                model.Hint = question.Hint;
                model.CurrentQuestion = question;
                return View(model);
            }
        }
        public IActionResult Result(int score)
        {
            ViewBag.Score = score;
            return View();
        }
        public async Task<IActionResult> EditeProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = user.UserName,
                Name = user.Name
            };

            return View(applicationUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditeProfile(ApplicationUser applicationUser)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            // تحديث البيانات مباشرة
            user.Name = applicationUser.Name;
            // user.UserName = applicationUser.UserName; (إذا أردت)

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(applicationUser);
            }
            else
            {
                TempData["SuccessMessage"] = "تم تحديث بياناتك بنجاح";
            }

            return View(user);
        }

    }
}















