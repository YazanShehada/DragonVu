using DragonVu.Application.Features.Quiz.Responses;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DragonVu.Application.Features.Quiz.Command.SubmitQuizStep;

public sealed class SubmitQuizStepCommandHandler
    : ICommandHandler<SubmitQuizStepCommand, QuizStepResult>
{
    private const int PointsPerCorrectAnswer = 10;

    private readonly AppDbContext _context;

    public SubmitQuizStepCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QuizStepResult> HandleAsync(
        SubmitQuizStepCommand command,
        CancellationToken cancellationToken = default)
    {
        var model = command.Model;

        if (model.QuestionIds.Count == 0
            || model.CurrentIndex < 0
            || model.CurrentIndex >= model.QuestionIds.Count)
        {
            return new QuizStepResult(null, null);
        }

        var questionId = model.QuestionIds[model.CurrentIndex];
        var question = await _context.questions
            .FirstOrDefaultAsync(q => q.Id == questionId, cancellationToken);

        if (question is null)
        {
            return new QuizStepResult(null, null);
        }

        model.CurrentQuestion = question;

        // The user asked to move on after reading the hint.
        if (command.Continue)
        {
            return await AdvanceAsync(command, model, cancellationToken);
        }

        if (model.SelectedAnswer == question.CorrectAnswer)
        {
            model.Score += PointsPerCorrectAnswer;
            return await AdvanceAsync(command, model, cancellationToken);
        }

        // Wrong answer: stop and reveal the hint.
        model.ShowHint = true;
        model.Hint = question.Hint;
        model.CurrentQuestion = question;

        return new QuizStepResult(model, null);
    }

    private async Task<QuizStepResult> AdvanceAsync(
        SubmitQuizStepCommand command,
        StepQuizVM model,
        CancellationToken cancellationToken)
    {
        model.CurrentIndex++;

        if (model.CurrentIndex >= model.QuestionIds.Count)
        {
            await SaveResultAsync(command, model, cancellationToken);
            return new QuizStepResult(null, model.Score);
        }

        model.CurrentQuestion = await _context.questions
            .FirstAsync(q => q.Id == model.QuestionIds[model.CurrentIndex], cancellationToken);

        model.ShowHint = false;
        model.Hint = string.Empty;
        model.SelectedAnswer = null;

        return new QuizStepResult(model, null);
    }

    private async Task SaveResultAsync(
        SubmitQuizStepCommand command,
        StepQuizVM model,
        CancellationToken cancellationToken)
    {
        var result = new Result
        {
            UserId = command.UserId,
            SubjectId = model.SubjectId,
            Score = model.Score,
            CreatedAt = DateTime.Now,
            Name = command.Name,
            UserName = command.UserName
        };

        _context.Add(result);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
