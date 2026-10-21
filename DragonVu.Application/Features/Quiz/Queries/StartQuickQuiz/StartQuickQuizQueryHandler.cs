using DragonVu.Application.Features.Quiz.Responses;
using DragonVu.Data;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Quiz.Queries.StartQuickQuiz;

public sealed class StartQuickQuizQueryHandler
    : IQueryHandler<StartQuickQuizQuery, StepQuizVM?>
{
    private const int QuestionCount = 10;

    private readonly AppDbContext _context;

    public StartQuickQuizQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StepQuizVM?> HandleAsync(
        StartQuickQuizQuery query,
        CancellationToken cancellationToken = default)
    {
        var questionIds = await _context.questions
            .Where(q => q.SubjectId == query.SubjectId)
            .OrderBy(q => Guid.NewGuid())
            .Take(QuestionCount)
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        if (questionIds.Count == 0)
        {
            return null;
        }

        var firstQuestion = await _context.questions
            .FirstAsync(q => q.Id == questionIds[0], cancellationToken);

        return new StepQuizVM
        {
            QuestionIds = questionIds,
            CurrentIndex = 0,
            Score = 0,
            SubjectId = query.SubjectId,
            CurrentQuestion = firstQuestion
        };
    }
}
