using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Questions.Queries.GetPendingAddQuestions;

public sealed class GetPendingAddQuestionsQueryHandler
    : IQueryHandler<GetPendingAddQuestionsQuery, IReadOnlyList<Question>>
{
    private readonly AppDbContext _context;

    public GetPendingAddQuestionsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Question>> HandleAsync(
        GetPendingAddQuestionsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _context.questions
            .Where(q => q.Status == QuestionStatus.PendingAdd)
            .ToListAsync(cancellationToken);
    }
}
