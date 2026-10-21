using DragonVu.Application.Features.Statistics.Responses;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Statistics.Queries.GetManageUsers;

public sealed record GetManageUsersQuery : IQuery<StatisticsVM>;
