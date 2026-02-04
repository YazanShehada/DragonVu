using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace DragonVu.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        // رابط صورة السؤال
        [Required]
        public string ImageUrl { get; set; } = null!;

        // رقم الإجابة الصحيحة (1 إلى 4)
        [Range(1, 4)]
        public int CorrectAnswer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
