using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Subjects.Queries.GetSubjects;

public sealed class GetSubjectsQueryHandler
    : IQueryHandler<GetSubjectsQuery, IReadOnlyList<Subject>>
{
    private readonly AppDbContext _context;

    public GetSubjectsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Subject>> HandleAsync(
        GetSubjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _context.subjects.ToListAsync(cancellationToken);
    }
}
