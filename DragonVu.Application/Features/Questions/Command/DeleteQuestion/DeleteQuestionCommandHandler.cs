using DragonVu.Application.Features.Questions.Services;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.DeleteQuestion;

public sealed class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IQuestionImageService _images;

    public DeleteQuestionCommandHandler(AppDbContext context, IQuestionImageService images)
    {
        _context = context;
        _images = images;
    }

    public async Task<bool> HandleAsync(
        DeleteQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        var question = await _context.questions.FindAsync([command.Id], cancellationToken);

        if (question is null)
        {
            return false;
        }

        if (command.IsAdmin)
        {
            await _images.DeleteAsync(question.ImageUrl);
            _context.questions.Remove(question);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        if (command.IsEditor)
        {
            question.Status = QuestionStatus.PendingDelete;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
