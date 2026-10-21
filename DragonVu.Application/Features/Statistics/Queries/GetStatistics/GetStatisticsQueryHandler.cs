using DragonVu.Application.Features.Statistics.Responses;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Statistics.Queries.GetStatistics;

public sealed class GetStatisticsQueryHandler
    : IQueryHandler<GetStatisticsQuery, StatisticsVM>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetStatisticsQueryHandler(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<StatisticsVM> HandleAsync(
        GetStatisticsQuery query,
        CancellationToken cancellationToken = default)
    {
        var results = await _context.results
            .Include(r => r.User)
            .Include(r => r.Subject)
            .ToListAsync(cancellationToken);

        var users = await _userManager.Users.ToListAsync(cancellationToken);

        var latestUsers = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new StatisticsVM
        {
            results = results,
            allUsers = users,
            UserCount = users.Count,
            latestUsers = latestUsers
        };
    }
}
