namespace DragonVu.Models
{
    public class StepQuizVM
    {
        public List<int> QuestionIds { get; set; }

        public int CurrentIndex { get; set; }

        public int? SelectedAnswer { get; set; }

        public int Score { get; set; }

        public int SubjectId { get; set; }

        public bool ShowHint { get; set; } = false;
        public string Hint { get; set; } = string.Empty;


        public Question CurrentQuestion { get; set; }
    }
}
