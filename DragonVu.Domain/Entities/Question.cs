using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DragonVu.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        public int SubjectId { get; set; }
        [ValidateNever]
        public Subject? Subject { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; } = "/images/default-question.png";

        [Range(1, 4)]
        public int CorrectAnswer { get; set; }

        public string Text { get; set; } = null!;

        // answers
        public string? AnswerA { get; set; } = null;
        public string? AnswerB { get; set; } = null;
        public string? AnswerC { get; set; } = null;
        public string? AnswerD { get; set; } = null;
        //

        public string Hint { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public QuestionStatus Status { get; set; } = QuestionStatus.PendingAdd;

        public string? CreatedById { get; set; }

        public string? ApprovedById { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}
