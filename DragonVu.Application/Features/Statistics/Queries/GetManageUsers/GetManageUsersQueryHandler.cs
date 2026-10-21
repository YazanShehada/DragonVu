using DragonVu.Application.Features.Statistics.Responses;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Statistics.Queries.GetManageUsers;

public sealed class GetManageUsersQueryHandler
    : IQueryHandler<GetManageUsersQuery, StatisticsVM>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetManageUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<StatisticsVM> HandleAsync(
        GetManageUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);

        return new StatisticsVM
        {
            allUsers = users,
            UserCount = users.Count
        };
    }
}
