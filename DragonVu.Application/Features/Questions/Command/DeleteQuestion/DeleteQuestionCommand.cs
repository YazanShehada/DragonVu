using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.DeleteQuestion;

public sealed record DeleteQuestionCommand(int Id, bool IsAdmin, bool IsEditor) : ICommand<bool>;
