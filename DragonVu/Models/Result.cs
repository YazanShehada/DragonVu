using DragonVu.Enums;

using System.ComponentModel.DataAnnotations;

namespace DragonVu.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        // المستخدم
        //    [Required]
        // public string UserId { get; set; } = null!;
        //  public ApplicationUser User { get; set; } = null!;

        // المادة
        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        // نوع الكويز
        [Required]
        public QuizType QuizType { get; set; }

        // النتيجة
        [Range(0, int.MaxValue)]
        public int Score { get; set; }

        [Range(1, int.MaxValue)]
        public int TotalScore { get; set; }

        // وقت الإنهاء
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
