using DragonVu.Models;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Subjects.Queries.GetSubjects;

public sealed record GetSubjectsQuery : IQuery<IReadOnlyList<Subject>>;
