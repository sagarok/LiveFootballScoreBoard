using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoreboard.Models;

namespace Scoreboard;

public interface IScoreboardService
{
    public Task<Guid> StartMatchAsync(string homeTeam, string awayTeam, CancellationToken cancellation = default);
    public Task FinishMatchAsync(Guid matchId, CancellationToken cancellation = default);
    public Task UpdateScoreAsync(Guid matchId, CancellationToken cancellation = default);
    public Task<IEnumerable<SummaryItem>> GetLiveSummaryAsync(CancellationToken cancellation = default);
}
