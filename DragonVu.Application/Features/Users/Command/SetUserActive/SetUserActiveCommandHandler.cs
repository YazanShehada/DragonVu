using DragonVu.Data;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Users.Command.SetUserActive;

public sealed class SetUserActiveCommandHandler
    : ICommandHandler<SetUserActiveCommand, bool>
{
    private readonly AppDbContext _context;

    public SetUserActiveCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(
        SetUserActiveCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([command.UserId], cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.IsActive = command.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
