using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Questions.Queries.GetPendingDeleteQuestions;

public sealed class GetPendingDeleteQuestionsQueryHandler
    : IQueryHandler<GetPendingDeleteQuestionsQuery, IReadOnlyList<Question>>
{
    private readonly AppDbContext _context;

    public GetPendingDeleteQuestionsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Question>> HandleAsync(
        GetPendingDeleteQuestionsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _context.questions
            .Where(q => q.Status == QuestionStatus.PendingDelete)
            .ToListAsync(cancellationToken);
    }
}
