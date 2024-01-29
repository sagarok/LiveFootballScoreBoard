using Scoreboard.Models;

namespace Scoreboard
{
    public class InMemoryScoreboardService : IScoreboardService
    {
        private readonly Dictionary<Guid, MatchInfo> _liveMatches = new();
        private readonly TimeProvider _timeProvider;
        private readonly ReaderWriterLockSlim _locker = new();

        public InMemoryScoreboardService(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public Task<Guid> StartMatchAsync(string homeTeam, string awayTeam, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(homeTeam))
                throw new ArgumentException("Team name must not be empty", nameof(homeTeam));
            if (string.IsNullOrWhiteSpace(awayTeam))
                throw new ArgumentException("Team name must not be empty", nameof(awayTeam));

            try
            {
                _locker.EnterWriteLock();

                if (MatchExists(homeTeam, awayTeam))
                {
                    throw new ArgumentException("There is already a match with these teams");
                }

                var newMatch = new MatchInfo(homeTeam, awayTeam, startedAt: _timeProvider.GetUtcNow());
                _liveMatches[newMatch.Id] = newMatch;

                return Task.FromResult(newMatch.Id);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public Task FinishMatchAsync(Guid matchId, CancellationToken cancellation = default)
        {
            try
            {
                _locker.EnterWriteLock();

                if (!_liveMatches.ContainsKey(matchId))
                {
                    throw new ArgumentOutOfRangeException(nameof(matchId));
                }

                _liveMatches.Remove(matchId);
            }
            finally
            {
                _locker.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<SummaryItem>> GetLiveSummaryAsync(CancellationToken cancellation = default)
        {
            try
            {
                _locker.EnterReadLock();
                var result = _liveMatches.Values
                    .OrderByDescending(v => v.HomeScore + v.AwayScore)
                    .ThenByDescending(v => v.StartedAt)
                    .Select(v => new SummaryItem(v.HomeName, v.HomeScore, v.AwayName, v.AwayScore))
                    .ToArray();

                return Task.FromResult<IEnumerable<SummaryItem>>(result);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public Task UpdateScoreAsync(Guid matchId, int homeScore, int awayScore, CancellationToken cancellation = default)
        {
            try
            {
                _locker.EnterWriteLock();


                if (!_liveMatches.TryGetValue(matchId, out var match))
                {
                    throw new ArgumentOutOfRangeException(nameof(matchId));
                }

                if (match.HomeScore > homeScore)
                {
                    throw new ArgumentOutOfRangeException(nameof(homeScore));
                }

                if (match.AwayScore > awayScore)
                {
                    throw new ArgumentOutOfRangeException(nameof(awayScore));
                }

                match.HomeScore = homeScore;
                match.AwayScore = awayScore;
            }
            finally
            {
                _locker.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        private bool MatchExists(string homeName, string awayName)
            => _liveMatches.Values.Any(match =>
            match.HomeName.Equals(homeName, StringComparison.InvariantCultureIgnoreCase) ||
            match.HomeName.Equals(awayName, StringComparison.InvariantCultureIgnoreCase) ||
            match.AwayName.Equals(homeName, StringComparison.InvariantCultureIgnoreCase) ||
            match.AwayName.Equals(awayName, StringComparison.InvariantCultureIgnoreCase));
    }
}
