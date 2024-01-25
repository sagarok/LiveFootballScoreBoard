using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoreboard.Models;

namespace Scoreboard
{
    public class ScoreboardService : IScoreboardService
    {
        public Task FinishMatchAsync(Guid matchId, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SummaryItem>> GetLiveSummaryAsync(CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> StartMatchAsync(string homeTeam, string awayTeam, CancellationToken cancellation = default)
        {
            var result = Guid.NewGuid();
            return Task.FromResult(result);
        }

        public Task UpdateScoreAsync(Guid matchId, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}
