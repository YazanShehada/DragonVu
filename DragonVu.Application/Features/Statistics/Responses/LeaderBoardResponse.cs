using System;
using System.Collections.Generic;
using System.Text;

namespace DragonVu.Application.Features.Statistics.Responses
{
    public class LeaderBoardResponse
    {
        public required string StudentName { get; set; }

        public required int Score { get; set; } 

        public required string  SubjectName { get; set; }

    }
}
