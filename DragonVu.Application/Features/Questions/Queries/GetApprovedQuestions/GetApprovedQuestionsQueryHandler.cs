using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Questions.Queries.GetApprovedQuestions;

public sealed class GetApprovedQuestionsQueryHandler
    : IQueryHandler<GetApprovedQuestionsQuery, IReadOnlyList<Question>>
{
    private readonly AppDbContext _context;

    public GetApprovedQuestionsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Question>> HandleAsync(
        GetApprovedQuestionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var questions = _context.questions
            .Where(q => q.Status == QuestionStatus.Approved);

        if (query.SubjectId.HasValue)
        {
            questions = questions.Where(q => q.SubjectId == query.SubjectId.Value);
        }

        return await questions.ToListAsync(cancellationToken);
    }
}
