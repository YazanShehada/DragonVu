using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Questions.Queries.GetQuestionsForEditing;

public sealed class GetQuestionsForEditingQueryHandler
    : IQueryHandler<GetQuestionsForEditingQuery, IReadOnlyList<Question>>
{
    private readonly AppDbContext _context;

    public GetQuestionsForEditingQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Question>> HandleAsync(
        GetQuestionsForEditingQuery query,
        CancellationToken cancellationToken = default)
    {
        var questions = _context.questions
            .Where(q => q.CreatedById == query.UserId);

        if (query.SubjectId.HasValue)
        {
            questions = questions.Where(q => q.SubjectId == query.SubjectId.Value);
        }

        return await questions.ToListAsync(cancellationToken);
    }
}
