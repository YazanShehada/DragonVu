using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Users.Command.SetUserActive;

public sealed record SetUserActiveCommand(string UserId, bool IsActive) : ICommand<bool>;
