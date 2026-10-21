using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.ReviewQuestion;

/// <summary>
/// Approves or rejects a question that is waiting for approval.
/// </summary>
public sealed record ReviewQuestionCommand(int QuestionId, bool Approve) : ICommand<bool>;
