using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.ReviewQuestionDeletion;

/// <summary>
/// Approves (deletes the question) or rejects a pending delete request.
/// </summary>
public sealed record ReviewQuestionDeletionCommand(int QuestionId, bool Approve)
    : ICommand<bool>;
