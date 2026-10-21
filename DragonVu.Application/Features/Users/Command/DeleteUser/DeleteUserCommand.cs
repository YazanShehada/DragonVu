using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Users.Command.DeleteUser;

public sealed record DeleteUserCommand(string UserId) : ICommand<bool>;
