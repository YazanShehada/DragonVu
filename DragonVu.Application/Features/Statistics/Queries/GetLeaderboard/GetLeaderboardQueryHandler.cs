using DragonVu.Application.Features.Statistics.Responses;
using DragonVu.Data;
using DragonVu.Models;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DragonVu.Application.Features.Statistics.Queries.GetLeaderboard
{
    public class GetLeaderboardQueryHandler(AppDbContext context , UserManager<ApplicationUser> userManager) : IQueryHandler<GetLeaderboardQuery, List<LeaderBoardResponse>>
    {
      
        public async Task<List<LeaderBoardResponse>> HandleAsync(GetLeaderboardQuery message, CancellationToken cancellationToken = default)
        {
            var leaderboard = await context.results
                .Include(r => r.User)
                .Include(r => r.Subject)
                .OrderByDescending(r => r.Score)
                .Select(r => new LeaderBoardResponse
                {
                    StudentName = r.User.Name,
                    Score = r.Score,
                    SubjectName = r.Subject.Name ,

                })
                .Take(10)
                .ToListAsync(cancellationToken);
            return leaderboard;
        }
    }
}
