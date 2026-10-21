using DragonVu.Application.Features.Quiz.Responses;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Quiz.Command.SubmitQuizStep;

public sealed record SubmitQuizStepCommand(
    StepQuizVM Model,
    bool Continue,
    string UserId,
    string Name,
    string UserName) : ICommand<QuizStepResult>;
