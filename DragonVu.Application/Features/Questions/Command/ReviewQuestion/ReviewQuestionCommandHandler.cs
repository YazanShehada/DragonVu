using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.ReviewQuestion;

public sealed class ReviewQuestionCommandHandler
    : ICommandHandler<ReviewQuestionCommand, bool>
{
    private readonly AppDbContext _context;

    public ReviewQuestionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(
        ReviewQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        var question = await _context.questions.FindAsync([command.QuestionId], cancellationToken);

        if (question is null)
        {
            return false;
        }

        question.Status = command.Approve
            ? QuestionStatus.Approved
            : QuestionStatus.Rejected;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
