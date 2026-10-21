namespace DragonVu.Application.Features.Quiz.Responses;

/// <summary>
/// Result of a quiz step. <see cref="Model"/> is null when the quiz is finished,
/// and <see cref="CompletedScore"/> carries the final score.
/// </summary>
public sealed record QuizStepResult(StepQuizVM? Model, int? CompletedScore);
