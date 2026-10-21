using DragonVu.Application.Features.Statistics.Responses;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Statistics.Queries.GetManagementDashboard;

public sealed record GetManagementDashboardQuery : IQuery<ManagementDashboardResponse>;
