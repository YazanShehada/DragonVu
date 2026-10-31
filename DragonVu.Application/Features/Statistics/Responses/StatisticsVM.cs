using DragonVu.Models;

namespace DragonVu.Application.Features.Statistics.Responses;

public class StatisticsVM
{
    public List<ApplicationUser> allUsers { get; set; } = [];

    public int PageNumber { get; set; } = 1;

    public int TotalPagesNumber { get; set; }

    public int UserCount { get; set; }

    public List<ApplicationUser> latestUsers { get; set; } = [];

    public List<Result> results { get; set; } = [];
}
