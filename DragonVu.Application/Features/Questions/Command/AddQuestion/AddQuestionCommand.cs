using DragonVu.Application.Features.Questions.Responses;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DragonVu.Application.Features.Questions.Command.AddQuestion;

public sealed record AddQuestionCommand(
    int SubjectId,
    string Text,
    string? AnswerA,
    string? AnswerB,
    string? AnswerC,
    string? AnswerD,
    int CorrectAnswer,
    string? Hint,
    IFormFile? Image,
    string UserId,
    bool IsAdmin,
    bool IsEditor) : ICommand<QuestionMutationResult>;
