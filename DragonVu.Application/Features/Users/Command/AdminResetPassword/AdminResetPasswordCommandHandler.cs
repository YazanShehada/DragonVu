using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace DragonVu.Application.Features.Users.Command.AdminResetPassword;

public class AdminResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
: ICommandHandler<AdminResetPasswordCommand, bool>
{
    public async Task<bool> HandleAsync(AdminResetPasswordCommand message, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(message.userName);

        if (user == null)
        {
            return false;
        }
        else
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, token, message.newPassword);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}