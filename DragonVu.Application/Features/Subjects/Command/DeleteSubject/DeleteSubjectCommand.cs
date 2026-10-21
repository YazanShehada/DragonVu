using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Subjects.Command.DeleteSubject;

public sealed record DeleteSubjectCommand(int Id) : ICommand<bool>;
