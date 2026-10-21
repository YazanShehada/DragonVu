using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Subjects.Command.AddSubject;

public sealed record AddSubjectCommand(int Id, string Name, int Year) : ICommand<int>;
