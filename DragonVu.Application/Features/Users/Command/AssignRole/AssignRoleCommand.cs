using DragonVu.Application.Features.Users.Responses;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Users.Command.AssignRole;

public sealed record AssignRoleCommand(string? Username, string? Role)
    : ICommand<AssignRoleResult>;
