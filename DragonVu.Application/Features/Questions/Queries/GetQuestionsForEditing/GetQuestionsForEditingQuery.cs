using DragonVu.Models;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Questions.Queries.GetQuestionsForEditing;

public sealed record GetQuestionsForEditingQuery(string UserId, int? SubjectId)
    : IQuery<IReadOnlyList<Question>>;
