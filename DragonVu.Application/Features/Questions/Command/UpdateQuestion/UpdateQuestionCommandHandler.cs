using DragonVu.Application.Features.Questions.Responses;
using DragonVu.Application.Features.Questions.Services;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Questions.Command.UpdateQuestion;

public sealed class UpdateQuestionCommandHandler
    : ICommandHandler<UpdateQuestionCommand, QuestionMutationResult>
{
    private readonly AppDbContext _context;
    private readonly IQuestionImageService _images;

    public UpdateQuestionCommandHandler(AppDbContext context, IQuestionImageService images)
    {
        _context = context;
        _images = images;
    }

    public async Task<QuestionMutationResult> HandleAsync(
        UpdateQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        var question = await _context.questions
            .FirstOrDefaultAsync(
                q => q.Id == command.Id && q.CreatedById == command.UserId,
                cancellationToken);

        if (question is null)
        {
            return QuestionMutationResult.NotFound();
        }

        if (command.SubjectId != 0)
        {
            question.SubjectId = command.SubjectId;
        }

        question.AnswerA = command.AnswerA;
        question.AnswerB = command.AnswerB;
        question.AnswerC = command.AnswerC;
        question.AnswerD = command.AnswerD;
        question.CorrectAnswer = command.CorrectAnswer;
        question.Hint = command.Hint ?? string.Empty;
        question.Text = command.Text ?? string.Empty;

        if (command.Image is { Length: > 0 })
        {
            if (!_images.IsSupported(command.Image.FileName))
            {
                return QuestionMutationResult.Failed("Invalid image format");
            }

            var previousImageUrl = question.ImageUrl;
            question.ImageUrl = await _images.SaveAsync(command.Image, question.SubjectId, cancellationToken);
            await _images.DeleteAsync(previousImageUrl);
        }

        if (command.IsEditor)
        {
            question.Status = QuestionStatus.PendingAdd;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return QuestionMutationResult.Ok(question.Id);
    }
}
