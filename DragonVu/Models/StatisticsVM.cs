namespace DragonVu.Models
{
    public class StatisticsVM
    {
        public List<ApplicationUser> allUsers { get; set; }

        public int UserCount { get; set; }

        public List<ApplicationUser> latestUsers { get; set; }

        public List<Result> results { get; set; }




    }
}
