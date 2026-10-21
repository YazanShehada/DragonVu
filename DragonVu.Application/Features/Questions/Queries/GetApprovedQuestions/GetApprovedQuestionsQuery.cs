using DragonVu.Models;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Questions.Queries.GetApprovedQuestions;

public sealed record GetApprovedQuestionsQuery(int? SubjectId) : IQuery<IReadOnlyList<Question>>;
