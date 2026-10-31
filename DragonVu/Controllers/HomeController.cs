using DragonVu.Application.Features.Statistics.Queries.GetLeaderboard;
using DragonVu.Application.Features.Subjects.Queries.GetSubjects;
using DragonVu.Models;
using DragonVu.Web.Models.Response;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace DragonVu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQueryMediator _queryMediator;

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IQueryMediator queryMediator, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _queryMediator = queryMediator;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("StartQuiz", "Home");
            }
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> StartQuiz()
        {
            var subjects = await _queryMediator.QueryAsync(new GetSubjectsQuery());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");

            return View();
        }

        [Authorize]
        public async Task<IActionResult> leaderboard()
        {


            var leaderBoard = await _queryMediator.QueryAsync(new GetLeaderboardQuery());


            return View(leaderBoard);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
