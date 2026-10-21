using DragonVu.Application.Features.Statistics.Responses;
using LiteBus.Queries.Abstractions;

namespace DragonVu.Application.Features.Statistics.Queries.GetStatistics;

public sealed record GetStatisticsQuery : IQuery<StatisticsVM>;
