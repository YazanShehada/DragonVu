using DragonVu.Application.Features.Questions.Command.ReviewQuestion;
using DragonVu.Application.Features.Questions.Command.ReviewQuestionDeletion;
using DragonVu.Application.Features.Questions.Command.UpdateQuestion;
using DragonVu.Application.Features.Questions.Queries.GetApprovedQuestions;
using DragonVu.Application.Features.Questions.Queries.GetPendingAddQuestions;
using DragonVu.Application.Features.Questions.Queries.GetPendingDeleteQuestions;
using DragonVu.Application.Features.Statistics.Queries.GetManagementDashboard;
using DragonVu.Application.Features.Statistics.Queries.GetManageUsers;
using DragonVu.Application.Features.Statistics.Queries.GetStatistics;
using DragonVu.Application.Features.Subjects.Command.AddSubject;
using DragonVu.Application.Features.Subjects.Command.DeleteSubject;
using DragonVu.Application.Features.Subjects.Queries.GetSubjects;
using DragonVu.Application.Features.Users.Command.AssignRole;
using DragonVu.Application.Features.Users.Command.DeleteUser;
using DragonVu.Application.Features.Users.Command.SetUserActive;
using DragonVu.Application.Features.Users.Queries.GetUsersWithRoles;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonVu.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class ManagementController : Controller
    {
        private readonly ICommandMediator _commandMediator;
        private readonly IQueryMediator _queryMediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagementController(
            ICommandMediator commandMediator,
            IQueryMediator queryMediator,
            UserManager<ApplicationUser> userManager)
        {
            _commandMediator = commandMediator;
            _queryMediator = queryMediator;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Editor")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var dashboard = await _queryMediator.QueryAsync(new GetManagementDashboardQuery());

            ViewBag.NewQuestions = dashboard.NewQuestions;
            ViewBag.DeleteRequests = dashboard.DeleteRequests;
            ViewBag.ApprovedQuestions = dashboard.ApprovedQuestions;
            ViewBag.SubjectsCount = dashboard.SubjectsCount;
            ViewBag.UsersCount = dashboard.UsersCount.ToString();

            return View();
        }

        public async Task<IActionResult> AddRoles()
        {
            await SetUsersWithRolesViewBagAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoles(string? dummy = null)
        {
            var username = Request.Form["Username"].ToString();
            var role = Request.Form["Role"].ToString();

            var result = await _commandMediator.SendAsync(new AssignRoleCommand(username, role));

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("AddRoles");
            }

            TempData["ErrorMessage"] = result.Message;
            await SetUsersWithRolesViewBagAsync();

            return View("AddRoles");
        }

        [HttpGet]
        public async Task<IActionResult> AddSubjects()
        {
            ViewBag.Subjects = await _queryMediator.QueryAsync(new GetSubjectsQuery());

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubjects(Subject subject)
        {
            if (!ModelState.IsValid)
            {
                return View(subject);
            }

            await _commandMediator.SendAsync(
                new AddSubjectCommand(subject.Id, subject.Name, subject.Year));

            return RedirectToAction("AddSubjects");
        }

        public async Task<IActionResult> DeleteSubject()
        {
            ViewBag.Subjects = await _queryMediator.QueryAsync(new GetSubjectsQuery());

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            await _commandMediator.SendAsync(new DeleteSubjectCommand(id));

            return RedirectToAction("AddSubjects");
        }

        public async Task<IActionResult> AproveQuestions()
        {
            var pendingQuestions = await _queryMediator.QueryAsync(new GetPendingAddQuestionsQuery());

            return View(pendingQuestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AproveQuestions(int QuestionId, string actionType)
        {
            if (actionType is "approve" or "reject")
            {
                await _commandMediator.SendAsync(
                    new ReviewQuestionCommand(QuestionId, actionType == "approve"));
            }

            return RedirectToAction("AproveQuestions");
        }

        public async Task<IActionResult> DeleteRequests()
        {
            var pendingDeleteQuestions = await _queryMediator.QueryAsync(new GetPendingDeleteQuestionsQuery());

            return View(pendingDeleteQuestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequests(int QuestionId, string actionType)
        {
            if (actionType is "approve" or "reject")
            {
                await _commandMediator.SendAsync(
                    new ReviewQuestionDeletionCommand(QuestionId, actionType == "approve"));
            }

            return RedirectToAction("DeleteRequests");
        }

        public async Task<IActionResult> ShowQuestions(int? SubjectId)
        {
            var subjects = await _queryMediator.QueryAsync(new GetSubjectsQuery());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name", SubjectId);

            var questions = await _queryMediator.QueryAsync(new GetApprovedQuestionsQuery(SubjectId));

            return View(questions);
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
                return RedirectToAction("ShowQuestions");
            }

            TempData["SuccessMessage"] = "تم تعديل السؤال بنجاح ✅";
            TempData["UpdatedQuestionId"] = Id;

            return RedirectToAction("ShowQuestions");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StatisticsPage()
        {
            var statistics = await _queryMediator.QueryAsync(new GetStatisticsQuery());

            return View(statistics);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var statistics = await _queryMediator.QueryAsync(new GetManageUsersQuery());

            return View(statistics);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUsers(string id, string action)
        {
            switch (action)
            {
                case "Activate":
                    await _commandMediator.SendAsync(new SetUserActiveCommand(id, true));
                    break;

                case "Deactivate":
                    await _commandMediator.SendAsync(new SetUserActiveCommand(id, false));
                    break;

                case "Delete":
                    await _commandMediator.SendAsync(new DeleteUserCommand(id));
                    break;
            }

            return RedirectToAction("StatisticsPage");
        }

        private async Task SetUsersWithRolesViewBagAsync()
        {
            ViewBag.UsersWithRoles = await _queryMediator.QueryAsync(new GetUsersWithRolesQuery());
        }
    }
}
