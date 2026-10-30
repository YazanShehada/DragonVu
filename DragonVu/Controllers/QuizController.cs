using DragonVu.Application.Features.Profile.Command.UpdateProfile;
using DragonVu.Application.Features.Questions.Command.AddQuestion;
using DragonVu.Application.Features.Questions.Command.DeleteQuestion;
using DragonVu.Application.Features.Questions.Command.UpdateQuestion;
using DragonVu.Application.Features.Questions.Queries.GetQuestionsForEditing;
using DragonVu.Application.Features.Quiz.Command.SubmitQuizStep;
using DragonVu.Application.Features.Quiz.Queries.StartQuickQuiz;
using DragonVu.Application.Features.Quiz.Responses;
using DragonVu.Application.Features.Subjects.Queries.GetSubjects;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonVu.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ICommandMediator _commandMediator;
        private readonly IQueryMediator _queryMediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(
            ICommandMediator commandMediator,
            IQueryMediator queryMediator,
            UserManager<ApplicationUser> userManager)
        {
            _commandMediator = commandMediator;
            _queryMediator = queryMediator;
            _userManager = userManager;
        }

        

        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> AddQuestion()
        {
            await SetSubjectsViewBagAsync();

            return View(new Question());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> AddQuestion(Question question, IFormFile? QuestionImage)
        {
            if (!ModelState.IsValid)
            {
                await SetSubjectsViewBagAsync();
                return View(question);
            }

            var result = await _commandMediator.SendAsync(new AddQuestionCommand(
                question.SubjectId,
                question.Text,
                question.AnswerA,
                question.AnswerB,
                question.AnswerC,
                question.AnswerD,
                question.CorrectAnswer,
                question.Hint,
                QuestionImage,
                _userManager.GetUserId(User)!,
                User.IsInRole("Admin"),
                User.IsInRole("Editor")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Invalid image format");
                await SetSubjectsViewBagAsync();
                return View(question);
            }

            TempData["SuccessMessage"] = User.IsInRole("Admin")
                ? "تم إضافة السؤال واعتماده بنجاح ✅"
                : "تم إرسال السؤال للمراجعة ⏳";

            return RedirectToAction("AddQuestion");
        }

        public async Task<IActionResult> EditeQuestion(int? SubjectId)
        {
            await SetSubjectsViewBagAsync(SubjectId);

            var questions = await _queryMediator.QueryAsync(
                new GetQuestionsForEditingQuery(_userManager.GetUserId(User)!, SubjectId));

            return View(questions);
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
            var result = await _commandMediator.SendAsync(new UpdateQuestionCommand(
                Id,
                _userManager.GetUserId(User)!,
                User.IsInRole("Editor"),
                SubjectId,
                CorrectAnswer,
                Text,
                AnswerA,
                AnswerB,
                AnswerC,
                AnswerD,
                Hint,
                QuestionImage));

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Invalid image format");
                return RedirectToAction("EditeQuestion");
            }

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
        public async Task<IActionResult> DeleteQuestion(int Id)
        {
            await _commandMediator.SendAsync(new DeleteQuestionCommand(
                Id,
                User.IsInRole("Admin"),
                User.IsInRole("Editor")));

            var returnUrl = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("EditeQuestion");
        }

        [HttpGet]
        public async Task<IActionResult> QuickQuiz(int subjectId)
        {
            var model = await _queryMediator.QueryAsync(new StartQuickQuizQuery(subjectId));

            if (model is null)
            {
                return RedirectToAction("Index");
            }

            return View("QuickQuiz", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickQuiz(StepQuizVM model, string? Continue)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            var result = await _commandMediator.SendAsync(new SubmitQuizStepCommand(
                model,
                !string.IsNullOrEmpty(Continue),
                user.Id,
                user.Name,
                user.UserName ?? string.Empty));

            if (result.CompletedScore.HasValue)
            {
                return RedirectToAction("Result", new { score = result.CompletedScore.Value });
            }

            if (result.Model is null)
            {
                return NotFound();
            }

            return View(result.Model);
        }

        public IActionResult Result(int score)
        {
            ViewBag.Score = score;
            return View();
        }

        public async Task<IActionResult> EditeProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            var applicationUser = new ApplicationUser
            {
                UserName = user?.UserName,
                Name = user?.Name ?? string.Empty
            };

            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditeProfile(ApplicationUser applicationUser)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            var result = await _commandMediator.SendAsync(
                new UpdateProfileCommand(user.Id, applicationUser.Name));

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(applicationUser);
            }

            TempData["SuccessMessage"] = "تم تحديث بياناتك بنجاح";

            return View(user);
        }

        private async Task SetSubjectsViewBagAsync(object? selectedValue = null)
        {
            var subjects = await _queryMediator.QueryAsync(new GetSubjectsQuery());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name", selectedValue);
        }
    }
}
