
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Users.Command.AdminResetPassword;

public record AdminResetPasswordCommand(string userName, string newPassword) : ICommand<bool>;