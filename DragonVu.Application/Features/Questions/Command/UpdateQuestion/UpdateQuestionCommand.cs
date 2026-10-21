using DragonVu.Application.Features.Questions.Responses;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DragonVu.Application.Features.Questions.Command.UpdateQuestion;

public sealed record UpdateQuestionCommand(
    int Id,
    string UserId,
    bool IsEditor,
    int SubjectId,
    int CorrectAnswer,
    string? Text,
    string? AnswerA,
    string? AnswerB,
    string? AnswerC,
    string? AnswerD,
    string? Hint,
    IFormFile? Image) : ICommand<QuestionMutationResult>;
