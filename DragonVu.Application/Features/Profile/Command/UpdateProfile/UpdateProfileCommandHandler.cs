using DragonVu.Application.Features.Profile.Responses;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace DragonVu.Application.Features.Profile.Command.UpdateProfile;

public sealed class UpdateProfileCommandHandler
    : ICommandHandler<UpdateProfileCommand, UpdateProfileResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UpdateProfileResult> HandleAsync(
        UpdateProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);

        if (user is null)
        {
            return UpdateProfileResult.NotFound();
        }

        user.Name = command.Name ?? string.Empty;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded
            ? UpdateProfileResult.Ok()
            : UpdateProfileResult.Failed(result.Errors.Select(e => e.Description).ToList());
    }
}
