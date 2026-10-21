using DragonVu.Models;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Questions.Queries.GetPendingDeleteQuestions;

public sealed record GetPendingDeleteQuestionsQuery : IQuery<IReadOnlyList<Question>>;
