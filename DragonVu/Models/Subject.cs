using System.ComponentModel.DataAnnotations;

namespace DragonVu.Models
{
    public class Subject
    {

        [Key]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; } = null!;

        // السنة (1 أو 2 حالياً)

        public int Year { get; set; }
        public ICollection<Question> questions { get; set; }
        public ICollection<Result> results { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    }

}




