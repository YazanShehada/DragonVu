using DragonVu.Models;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Questions.Queries.GetPendingAddQuestions;

public sealed record GetPendingAddQuestionsQuery : IQuery<IReadOnlyList<Question>>;
