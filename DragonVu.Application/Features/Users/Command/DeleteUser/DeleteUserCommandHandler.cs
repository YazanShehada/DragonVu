using DragonVu.Data;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Users.Command.DeleteUser;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteUserCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([command.UserId], cancellationToken);

        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
