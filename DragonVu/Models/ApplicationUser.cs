using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DragonVu.Models
{
    public class ApplicationUser : IdentityUser
    {
        // الاسم الكامل للطالب
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        // سنة الطالب الحالية (اختياري)
        [Range(1, 2)]
        public int Year { get; set; } = 1;

        // Navigation Property للنتائج
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}