using DragonVu.Application.Features.Statistics.Responses;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Statistics.Queries.GetManagementDashboard;

public sealed class GetManagementDashboardQueryHandler
    : IQueryHandler<GetManagementDashboardQuery, ManagementDashboardResponse>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetManagementDashboardQueryHandler(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ManagementDashboardResponse> HandleAsync(
        GetManagementDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var newQuestions = await _context.questions
            .CountAsync(q => q.Status == QuestionStatus.PendingAdd, cancellationToken);

        var deleteRequests = await _context.questions
            .CountAsync(q => q.Status == QuestionStatus.PendingDelete, cancellationToken);

        var approvedQuestions = await _context.questions
            .CountAsync(q => q.Status == QuestionStatus.Approved, cancellationToken);

        var subjectsCount = await _context.subjects.CountAsync(cancellationToken);

        var users = await _userManager.Users.ToListAsync(cancellationToken);
        //var usersCount = await _userManager.Users..CountAsync(); 
        var staffCount = 0;
        // try to opmize this
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("User"))
            {
                staffCount++;
            }
        }

        return new ManagementDashboardResponse(
            newQuestions,
            deleteRequests,
            approvedQuestions,
            subjectsCount,
            staffCount);
    }
}
