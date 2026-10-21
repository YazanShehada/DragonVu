using DragonVu.Application.Features.Users.Responses;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Users.Queries.GetUsersWithRoles;

public sealed class GetUsersWithRolesQueryHandler
    : IQueryHandler<GetUsersWithRolesQuery, IReadOnlyList<UserWithRoleResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersWithRolesQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<UserWithRoleResponse>> HandleAsync(
        GetUsersWithRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        var result = new List<UserWithRoleResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("User"))
            {
                result.Add(new UserWithRoleResponse(
                    user.UserName ?? string.Empty,
                    roles.FirstOrDefault() ?? "لا يوجد"));
            }
        }

        return result;
    }
}
