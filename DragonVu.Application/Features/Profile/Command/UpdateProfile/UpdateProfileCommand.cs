using DragonVu.Application.Features.Profile.Responses;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Profile.Command.UpdateProfile;

public sealed record UpdateProfileCommand(string UserId, string? Name)
    : ICommand<UpdateProfileResult>;
