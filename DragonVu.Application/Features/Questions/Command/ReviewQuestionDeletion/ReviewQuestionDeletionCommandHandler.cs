using DragonVu.Application.Features.Questions.Services;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.ReviewQuestionDeletion;

public sealed class ReviewQuestionDeletionCommandHandler
    : ICommandHandler<ReviewQuestionDeletionCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IQuestionImageService _images;

    public ReviewQuestionDeletionCommandHandler(AppDbContext context, IQuestionImageService images)
    {
        _context = context;
        _images = images;
    }

    public async Task<bool> HandleAsync(
        ReviewQuestionDeletionCommand command,
        CancellationToken cancellationToken = default)
    {
        var question = await _context.questions.FindAsync([command.QuestionId], cancellationToken);

        if (question is null)
        {
            return false;
        }

        if (command.Approve)
        {
            await _images.DeleteAsync(question.ImageUrl);
            _context.questions.Remove(question);
        }
        else
        {
            question.Status = QuestionStatus.Approved;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
