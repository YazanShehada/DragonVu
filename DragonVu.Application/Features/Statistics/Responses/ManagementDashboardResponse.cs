namespace DragonVu.Application.Features.Statistics.Responses;

public sealed record ManagementDashboardResponse(
    int NewQuestions,
    int DeleteRequests,
    int ApprovedQuestions,
    int SubjectsCount,
    int UsersCount);
