using DragonVu.Application.Features.Users.Responses;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace DragonVu.Application.Features.Users.Command.AssignRole;

public sealed class AssignRoleCommandHandler
    : ICommandHandler<AssignRoleCommand, AssignRoleResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AssignRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<AssignRoleResult> HandleAsync(
        AssignRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var username = command.Username;
        var role = command.Role;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
        {
            return AssignRoleResult.Failed("اسم المستخدم أو الدور فارغ!");
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user is null)
        {
            return AssignRoleResult.Failed("المستخدم غير موجود!");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return AssignRoleResult.Failed("الدور غير موجود!");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await _userManager.AddToRoleAsync(user, role);

        return result.Succeeded
            ? AssignRoleResult.Success($"تم تعيين الدور '{role}' للمستخدم '{username}' بنجاح ✅")
            : AssignRoleResult.Failed("حدث خطأ أثناء تعيين الدور!");
    }
}
