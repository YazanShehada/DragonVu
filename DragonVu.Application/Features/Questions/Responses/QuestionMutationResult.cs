namespace DragonVu.Application.Features.Questions.Responses;

/// <summary>
/// Outcome of a question create/update operation.
/// </summary>
public sealed record QuestionMutationResult(
    bool Succeeded,
    int QuestionId = 0,
    string? Error = null,
    bool IsNotFound = false)
{
    public static QuestionMutationResult Ok(int questionId) => new(true, questionId);

    public static QuestionMutationResult Failed(string error) => new(false, 0, error);

    public static QuestionMutationResult NotFound() => new(false, 0, "Question not found.", true);
}
