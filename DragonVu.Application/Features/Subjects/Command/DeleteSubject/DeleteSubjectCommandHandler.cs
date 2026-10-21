using DragonVu.Data;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Subjects.Command.DeleteSubject;

public sealed class DeleteSubjectCommandHandler : ICommandHandler<DeleteSubjectCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteSubjectCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(
        DeleteSubjectCommand command,
        CancellationToken cancellationToken = default)
    {
        var subject = await _context.subjects.FindAsync([command.Id], cancellationToken);

        if (subject is null)
        {
            return false;
        }

        _context.subjects.Remove(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
