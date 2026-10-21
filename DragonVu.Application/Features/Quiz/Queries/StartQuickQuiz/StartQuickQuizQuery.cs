using DragonVu.Application.Features.Quiz.Responses;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Quiz.Queries.StartQuickQuiz;

public sealed record StartQuickQuizQuery(int SubjectId) : IQuery<StepQuizVM?>;
