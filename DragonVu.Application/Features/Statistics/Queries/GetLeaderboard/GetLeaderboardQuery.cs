using DragonVu.Application.Features.Statistics.Responses;
using LiteBus.Queries.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DragonVu.Application.Features.Statistics.Queries.GetLeaderboard
{
    public class GetLeaderboardQuery() : IQuery<List<LeaderBoardResponse>>;
}
