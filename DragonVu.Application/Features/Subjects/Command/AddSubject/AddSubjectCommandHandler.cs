using DragonVu.Application.Features.Questions.Services;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Subjects.Command.AddSubject;

public sealed class AddSubjectCommandHandler : ICommandHandler<AddSubjectCommand, int>
{
    private readonly AppDbContext _context;
    private readonly IQuestionImageService _images;

    public AddSubjectCommandHandler(AppDbContext context, IQuestionImageService images)
    {
        _context = context;
        _images = images;
    }

    public async Task<int> HandleAsync(
        AddSubjectCommand command,
        CancellationToken cancellationToken = default)
    {
        var subject = new Subject
        {
            Id = command.Id,
            Name = command.Name,
            Year = command.Year,
            CreatedAt = DateTime.Now
        };

        if (command.Id > 0)
        {
            _context.subjects.Update(subject);
        }
        else
        {
            _context.subjects.Add(subject);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _images.EnsureSubjectFolder(subject.Id);

        return subject.Id;
    }
}
