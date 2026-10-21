using DragonVu.Application.Features.Questions.Responses;
using DragonVu.Application.Features.Questions.Services;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;

namespace DragonVu.Application.Features.Questions.Command.AddQuestion;

public sealed class AddQuestionCommandHandler
    : ICommandHandler<AddQuestionCommand, QuestionMutationResult>
{
    private readonly AppDbContext _context;
    private readonly IQuestionImageService _images;

    public AddQuestionCommandHandler(AppDbContext context, IQuestionImageService images)
    {
        _context = context;
        _images = images;
    }

    public async Task<QuestionMutationResult> HandleAsync(
        AddQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = QuestionImageService.DefaultImageUrl;

        if (command.Image is { Length: > 0 })
        {
            if (!_images.IsSupported(command.Image.FileName))
            {
                return QuestionMutationResult.Failed("Invalid image format");
            }

            imageUrl = await _images.SaveAsync(command.Image, command.SubjectId, cancellationToken);
        }

        var question = new Question
        {
            SubjectId = command.SubjectId,
            Text = command.Text,
            AnswerA = command.AnswerA,
            AnswerB = command.AnswerB,
            AnswerC = command.AnswerC,
            AnswerD = command.AnswerD,
            CorrectAnswer = command.CorrectAnswer,
            Hint = command.Hint ?? string.Empty,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedById = command.UserId
        };

        if (command.IsAdmin)
        {
            question.Status = QuestionStatus.Approved;
            question.ApprovedAt = DateTime.UtcNow;
            question.ApprovedById = command.UserId;
        }
        else if (command.IsEditor)
        {
            question.Status = QuestionStatus.PendingAdd;
        }

        _context.questions.Add(question);
        await _context.SaveChangesAsync(cancellationToken);

        return QuestionMutationResult.Ok(question.Id);
    }
}
